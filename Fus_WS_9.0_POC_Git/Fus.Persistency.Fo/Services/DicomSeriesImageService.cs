using Dicom;
using Ws.Dicom.Interfaces.Entities;
using Dicom.Network;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.Fo.Entities;
using Ws.Dicom.Persistency.Fo.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Ws.Dicom.Persistency.Fo.Services
{
    class DicomSeriesImageService
    {
        class Request
        {
            public Series Series { get; set; }
            public IEnumerable<ImageDesc> ImagesDesc { get; set; }
        }

        private static readonly ILogger _logger = Log.ForContext<DicomSeriesImageService>();


        private readonly TransformBlock<Series, Request> _getImagesDesc;
        private readonly BatchBlock<Request> _batch;
        private readonly Timer _batchTimer;
        private readonly ActionBlock<Request[]> _getImage;

        private DicomSeriesImageService(Action<Image> progress, DicomSearchServiceSettings settings, CancellationToken ct)
        {
            _getImagesDesc = new TransformBlock<Series, Request>(async ser =>
            {
                try
                {
                    var desc = await GetImagesDescAsync(ser, settings, ct);
                    return new Request
                    {
                        Series = ser,
                        ImagesDesc = desc
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to get images desc for from series {SeriesInstanceUid}", ser.SeriesInstanceUid);
                    return new Request
                    {
                        Series = ser,
                        ImagesDesc = new List<ImageDesc>()
                    };
                }
            }, new ExecutionDataflowBlockOptions { CancellationToken = ct, MaxDegreeOfParallelism = settings.SeriesImageSettings.MaxParallelImageQueries });

            _getImage = new ActionBlock<Request[]>(async requests =>
            {
                try
                {
                    await GetImageAsync(requests, settings, ct, progress);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to get image");
                }

            }, new ExecutionDataflowBlockOptions { CancellationToken = ct, MaxDegreeOfParallelism = settings.SeriesImageSettings.MaxParallelMoveRequests });

            _batch = new BatchBlock<Request>(10);

            _getImagesDesc.LinkTo(_batch, new DataflowLinkOptions { PropagateCompletion = true });
            _batch.LinkTo(_getImage, new DataflowLinkOptions { PropagateCompletion = true });

            _batchTimer = new Timer(delegate { _batch.TriggerBatch(); });
            _batchTimer.Change(1000, 1000);
        }

        public static async Task GetSeriesImageAsync(GetSeriesImageRequest request, DicomSearchServiceSettings settings, CancellationToken ct)
        {
            var service = new DicomSeriesImageService(request.RaiseImageGot, settings, ct);
            await service.Go(request.Series, ct);
        }

        public async Task Go(IEnumerable<Series> series, CancellationToken ct)
        {
            foreach (var ser in series)
                await _getImagesDesc.SendAsync(ser, ct);

            _getImagesDesc.Complete();
            await _getImage.Completion;
        }

        private static ImageImp LoadImage(ImageDesc imageDesc)
        {
            var imagePath = CStoreScp.GetImageUri(imageDesc.SeriesInstanceUid, imageDesc.SopInstanceUid);

            if (!File.Exists(imagePath))
                return null;

            var dicomFile = DicomFile.Open(imagePath);

            return dicomFile.CreateImage();
        }

        private async Task<IEnumerable<ImageDesc>> GetImagesDescAsync(Series series, DicomSearchServiceSettings settings, CancellationToken ct)
        {
            var uid = series.SeriesInstanceUid;
            var max = settings.SeriesImageSettings.SeriesImageTakeImages;

            var result = new List<ImageDesc>();

            if (ct.IsCancellationRequested)
                return result;

            var request = DicomCFindRequest.CreateImageQuery(series.StudyInstanceUid, series.SeriesInstanceUid);

            CancellationTokenSource tooManySeriesCts = null;
            var cancellationLock = new object();

            request.OnResponseReceived += (req, resp) =>
            {
                if (ct.IsCancellationRequested || resp.Dataset == null)
                    return;

                if (result.Count >= settings.SeriesImageSettings.SeriesImageTakeImages)
                {
                    lock (cancellationLock)
                    {
                        if (!tooManySeriesCts.IsCancellationRequested)
                        {
                            tooManySeriesCts.Cancel();
                        }
                    }

                    if (tooManySeriesCts.IsCancellationRequested)
                        _logger.Debug("Cancelled Image query for series {Uid} after {Max}", uid, max);

                    return;
                }

                var imageDesc = resp.Dataset.CreateObject<ImageDesc>();
                if (imageDesc == null)
                {
                    _logger.Error("ImageDesc creation failed");
                    return;
                }

                lock (cancellationLock)
                {
                    result.Add(imageDesc);
                }

                _logger.Debug("Got imageDesc for series {Uid} on image {ImageUid}. Results: {Count}", uid, imageDesc.SopInstanceUid, result.Count);
            };

            using (tooManySeriesCts = new CancellationTokenSource())
            {
                using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, tooManySeriesCts.Token))
                {
                    _logger.Debug("Getting images desc for series {SeriesInstanceUid}", series.SeriesInstanceUid);

                    await DicomSearchService.FindAsync<ImageDesc>(request, settings, null, linkedCts.Token);

                    return result;
                }
            }
        }

        private async Task GetImageAsync(Request[] requests, DicomSearchServiceSettings settings, CancellationToken ct, Action<Image> progress)
        {
            var requestsByDicom = new Dictionary<DicomCMoveRequest, Request>(); // filled below

            DicomCMoveRequest.ResponseDelegate handler = (dicomReq, dicomResp) =>
            {
                if (ct.IsCancellationRequested || dicomResp.Status.State != DicomState.Success)
                    return;

                var request = requestsByDicom[dicomReq];
                var midImageDesc = request.ImagesDesc.OrderBy(id => id.InstanceNumber).ElementAt(request.ImagesDesc.Count() / 2);

                var image = LoadImage(midImageDesc);
                if (image == null)
                    return;

                if (request.Series.Orientation == FDCSeriesOrientation.eFDC_NO_ORIENTATION &&
                    image.Orientation != FDCSeriesOrientation.eFDC_NO_ORIENTATION)
                    request.Series.Orientation = image.Orientation;

                progress(image);
            };

            foreach (var request in requests)
            {
                var count = request.ImagesDesc.Count();
                if (count <= 0 )
                    continue;

                var midImageDesc = request.ImagesDesc.OrderBy(id => id.InstanceNumber).ElementAt(count / 2);
                var localImage = LoadImage(midImageDesc);
                if (localImage != null)
                {
                    _logger.Debug("no need to get image for series {SeriesInstanceUid}, it's on the disk", midImageDesc.SeriesInstanceUid);

                    if (request.Series.Orientation == FDCSeriesOrientation.eFDC_NO_ORIENTATION &&
                        localImage.Orientation != FDCSeriesOrientation.eFDC_NO_ORIENTATION)
                        request.Series.Orientation = localImage.Orientation;

                    progress(localImage);
                    continue;
                }

                var dicomRequest = midImageDesc.CreateCMoveRequest(settings.DicomSettings);

                dicomRequest.OnResponseReceived += handler;
                requestsByDicom[dicomRequest] = request;
            }

            if (requestsByDicom.Count == 0)
                return;

            _logger.Debug("Getting mid images");

            var client = settings.DicomSettings.CreateClient();
            await client.AddRequestsAsync(requestsByDicom.Keys);

            using (var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                timeoutCts.CancelAfter(settings.SeriesImageSettings.GetImageTimeoutMs * requestsByDicom.Count);
                await client.SendAsync(timeoutCts.Token);
            }
        }
    }
}
