using Dicom;
using Dicom.Imaging;
using Ws.Dicom.Interfaces.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    static class DicomFileExtensions
    {
        private static readonly ILogger _logger = Log.ForContext<DicomFile>();

        public static ImageImp CreateImage(this DicomFile dicomFile)
        {
            DicomImage dicomImg = null;

            try
            {
                dicomImg = new DicomImage(dicomFile.Dataset);
            }
            catch (DicomImagingException ex)
            {
                _logger.Warning(ex, "Failed to create Dicom image from file {path}", dicomFile.File.Name);
            }

            var image = dicomFile.Dataset.CreateObject(() => new ImageImp(dicomImg));
            if (image == null)
            {
                _logger.Error("Failed to create ImageImp object");
                return null;
            }

            try
            {
                var geom = new FrameGeometry(dicomFile.Dataset);

                switch (geom.Orientation)
                {
                    case FrameOrientation.Axial:
                        image.Orientation = FDCSeriesOrientation.eFDC_AXIAL;
                        break;
                    case FrameOrientation.Sagittal:
                        image.Orientation = FDCSeriesOrientation.eFDC_SAGITTAL;
                        break;
                    case FrameOrientation.Coronal:
                        image.Orientation = FDCSeriesOrientation.eFDC_CORONAL;
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to create geometry for image {uid} in series {seriesUid}", image.SopInstanceUid, image.SeriesInstanceUid);
            }

            return image;
        }
    }
}
