using Dicom;
using Dicom.Log;
using Dicom.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Services
{
    class CStoreScp : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        private const string StorageDir = @".\DICOM";

        private static readonly ILogger _logger = Log.ForContext<CStoreScp>();

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes = new DicomTransferSyntax[]
        {
               // Lossless
               DicomTransferSyntax.JPEGLSLossless,
               DicomTransferSyntax.JPEG2000Lossless,
               DicomTransferSyntax.JPEGProcess14SV1,
               DicomTransferSyntax.JPEGProcess14,
               DicomTransferSyntax.RLELossless,
               // Lossy
               DicomTransferSyntax.JPEGLSNearLossless,
               DicomTransferSyntax.JPEG2000Lossy,
               DicomTransferSyntax.JPEGProcess1,
               DicomTransferSyntax.JPEGProcess2_4,
               // Uncompressed
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
                {
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
        };
        public CStoreScp(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
        {
        }

        public static HashSet<string> AllowedCalledAE { get; } = new HashSet<string>();

        private static string StoragePath => Path.GetFullPath(StorageDir);

        /// <summary>
        /// Location of images for the specified series.
        /// </summary>
        /// <param name="seriesInstanceUid"></param>
        /// <returns></returns>
        public static string GetSeriesImagesUri(string seriesInstanceUid) => Path.Combine(StoragePath, seriesInstanceUid);

        /// <summary>
        /// Location of image for for the specified series and image instance.
        /// </summary>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="sopInstanceUid"></param>
        /// <returns></returns>
        public static string GetImageUri(string seriesInstanceUid, string sopInstanceUid) => Path.Combine(GetSeriesImagesUri(seriesInstanceUid), sopInstanceUid) + ".dcm";

        public static void StorageCleanup(double storageHiWaterMarkGb, double storageLoWaterMarkGb)
        {
            var path = StoragePath;

            _logger.Information("Checking for cleanup. The storage path is {StoragePath}", path);

            if (!Directory.Exists(path))
            {
                _logger.Information("{StoragePath} doesn't exist yet - no need for cleanup", path);
                return;
            }

            _logger.Debug("storageHiWaterMark= {storageHiWaterMarkGb}Gb, storageLoWaterMark= {storageLoWaterMarkGb}Gb", storageHiWaterMarkGb, storageLoWaterMarkGb);

            var storageHiWaterMark = (long)(storageHiWaterMarkGb * 1024 * 1024 * 1024);
            var storageLoWaterMark = (long)(storageLoWaterMarkGb * 1024 * 1024 * 1024);

            long storageSize = 0;
            object parallelLock = new object();

            var newFirst = new DirectoryInfo(path).EnumerateDirectories().ToArray().OrderByDescending(i => i.CreationTimeUtc);
            var survivers = new LinkedList<DirectoryInfo>();

            Parallel.ForEach(newFirst, di =>
            {
                var dirSize = di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                lock (parallelLock)
                {
                    storageSize += dirSize;
                    if (storageSize < storageLoWaterMark)
                        survivers.AddLast(di);
                }
            });

            if (storageSize <= storageHiWaterMark)
            {
                _logger.Information("The storage size is {size} - no cleanup needed", storageSize);
                return;
            }
            else
            {
                _logger.Information("The storage size is {size} - {bytesToDelete} bytes should be cleaned up", storageSize, storageSize - storageLoWaterMark);
            }

            var victims = newFirst.Except(survivers);

            Parallel.ForEach(victims, v =>
            {
                try
                {
                    v.Delete(true);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to delete directory {path}", v.FullName);
                }
            });

            _logger.Information("Cleanup done, {count} directories deleted", victims.Count());
        }

        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        public void OnConnectionClosed(Exception exception)
        {
        }

        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            try
            {
                var seriesUid = request.Dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                var instUid = request.SOPInstanceUID.UID;

                var imagePath = GetImageUri(seriesUid, instUid);
                var imageDir = Path.GetDirectoryName(imagePath);

                if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);

                request.File.Save(imagePath);

                return new DicomCStoreResponse(request, DicomStatus.Success);
            }
            catch (Exception ex)
            {
                return new DicomCStoreResponse(request, DicomStatus.ProcessingFailure);
            }
        }

        public void OnCStoreRequestException(string tempFileName, Exception e)
        {
            // let library handle logging and error response
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            if (!AllowedCalledAE.Contains(association.CalledAE))
            {
                return SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    DicomRejectReason.CalledAENotRecognized);
            }

            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification) pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None) pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
            }

            return SendAssociationAcceptAsync(association);
        }
    }
}
