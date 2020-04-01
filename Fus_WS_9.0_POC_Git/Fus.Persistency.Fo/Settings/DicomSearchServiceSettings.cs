using Ws.Dicom.Persistency.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Settings
{
    class DicomSearchServiceSettings
    {
        public SearchServiceSettingsBase BaseSettings = new SearchServiceSettingsBase();
        public DicomServerSettings DicomSettings { get; set; } = new DicomServerSettings();

        public DicomSeriesImageServiceSettings SeriesImageSettings { get; set; } = new DicomSeriesImageServiceSettings();
    }
}
