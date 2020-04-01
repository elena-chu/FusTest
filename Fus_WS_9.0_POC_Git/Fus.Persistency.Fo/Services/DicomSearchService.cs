using Dicom;
using Dicom.Network;
using Ws.Dicom.Interfaces.Entities;
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
    class DicomSearchService : SearchServiceBase
    {
        private static readonly ILogger _logger = Log.ForContext<DicomSearchService>();

        private readonly DicomSearchServiceSettings _settings;

        public DicomSearchService(DicomSearchServiceSettings settings) : base(settings.BaseSettings)
        {
            _settings = settings;
        }

        public override async Task FindStudiesAsync(FindStudiesRequest request, CancellationToken ct)
        {
            var query = DicomCFindRequest.CreateStudyQuery();

            var patientName = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.Family))
                patientName += request.Family;
            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                if (string.IsNullOrWhiteSpace(patientName))
                    patientName += '*';

                patientName += '^' + request.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(patientName))
                query.Dataset.AddOrUpdate(DicomTag.PatientName, patientName);

            if (!string.IsNullOrWhiteSpace(request.PatientId))
                query.Dataset.AddOrUpdate(DicomTag.PatientID, request.PatientId);

            if (!string.IsNullOrWhiteSpace(request.Description))
                query.Dataset.AddOrUpdate(DicomTag.StudyDescription, request.Description);

            if (request.From.HasValue || request.To.HasValue)
            {
                var from = request.From.GetValueOrDefault(DateTime.MinValue);
                var to = request.To.GetValueOrDefault(DateTime.MaxValue);

                query.Dataset.AddOrUpdate(DicomTag.StudyDate, new DicomDateRange(from, to));
            }

            query.Dataset.AddOrUpdate(DicomTag.StudyID, string.Empty);

            query.OnResponseReceived += (req, resp) =>
            {
                if (!ct.IsCancellationRequested && resp.Dataset != null)
                {
                    var study = resp.Dataset.CreateObject<Study>();

                    if (study == null)
                        _logger.Error("Study creation failed");
                    else
                        request.RaiseStudyFound(study);
                }
            };

            await FindAsync<Study>(query, _settings, _findStudiesSemaphore, ct);
        }

        internal static async Task FindAsync<U>(
            DicomCFindRequest request,
            DicomSearchServiceSettings serviceSettings,
            SemaphoreSlim requestSemaphore,
            CancellationToken ct)
        {
            // always add the encoding
            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            // add the dicom tags with empty values that should be included in the result of the QR Server
            request.Dataset.Fill<U>();

            var client = serviceSettings.DicomSettings.CreateClient();
            await client.AddRequestAsync(request);

            if (requestSemaphore != null)
                await requestSemaphore.WaitAsync(ct);

            try
            {
                if (!ct.IsCancellationRequested)
                    await client.SendAsync(ct);
            }
            finally
            {
                if (requestSemaphore != null)
                    requestSemaphore.Release();
            }
        }

        

        internal override async Task GetStudySeriesImpAsync(GetStudySeriesRequest request, CancellationToken ct)
        {
            var dicomRequest = DicomCFindRequest.CreateSeriesQuery(request.Study.StudyInstanceUid);

            dicomRequest.OnResponseReceived += (req, resp) =>
            {
                if (!ct.IsCancellationRequested && resp.Dataset != null)
                {
                    var series = resp.Dataset.CreateObject<Series>();

                    if (series == null)
                    {
                        _logger.Error("Series creation failed");
                    }
                    else
                    {
                        series.Study = request.Study;
                        request.RaiseSeriesGot(series);
                    }
                }
            };

            await FindAsync<Series>(dicomRequest, _settings, _getStudySeriesSemaphore, ct);
        }

        internal override async Task GetSeriesImageImpAsync(GetSeriesImageRequest request, CancellationToken ct)
        {
            await _getSeriesImageSemaphore.WaitAsync(ct);

            try
            {
                if (ct.IsCancellationRequested)
                    return;

                await DicomSeriesImageService.GetSeriesImageAsync(request, _settings, ct);
            }
            finally
            {
                _getSeriesImageSemaphore.Release();
            }
        }

        internal override async Task GetSeriesImagesImpAsync(GetSeriesImagesRequest request, CancellationToken ct)
        {
            var seriesByReq = new Dictionary<DicomCMoveRequest, Series>(); // filled below
            var remainingImagesByReq = new Dictionary<DicomCMoveRequest, int>(); // filled below
            var totalImages = request.Series.Sum(s => s.NumberOfSeriesRelatedInstances);

            object progressLock = new object();

            DicomCMoveRequest.ResponseDelegate handler = (req, resp) =>
            {
                lock (progressLock)
                {
                    remainingImagesByReq[req] = resp.Remaining;
                    var remainingImages = remainingImagesByReq.Sum(kvp => kvp.Value);
                    var progress = (double)(totalImages - remainingImages) / totalImages;

                    if (progress < 0.99)
                        request.RaiseProgress((int)(progress * 100));
                }

                if (resp.Remaining == 0)
                {
                    var series = seriesByReq[req];
                    series.ImagesUri = CStoreScp.GetSeriesImagesUri(series.SeriesInstanceUid);
                    request.RaiseSeriesDone(series);
                }
            };

            foreach (var series in request.Series)
            {
                var cmove = series.CreateCMoveRequest(_settings.DicomSettings);
                cmove.OnResponseReceived += handler;
                seriesByReq[cmove] = series;
                remainingImagesByReq[cmove] = series.NumberOfSeriesRelatedInstances;
            }

            var client = _settings.DicomSettings.CreateClient();
            await client.AddRequestsAsync(seriesByReq.Keys);
            await client.SendAsync(ct);

            request.RaiseProgress(100);
        }
    }
}
