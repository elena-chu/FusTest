using Ws.Extensions.Configuration.Patterns;
using Ws.Dicom.Persistency.Fo.Settings;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Configuration
{
    class FusPersistencyFoConfiguration : Configuration<FusPersistencyFoSettings>
    {
        private static readonly string[] _jsonFiles;

        static FusPersistencyFoConfiguration()
        {
            var loc = typeof(FusPersistencyFoConfiguration).Assembly.Location;
            
            _jsonFiles = new string[]
            {
                Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(loc), "Configuration", Path.GetFileName(loc)), "json"),
            };
        }

        public FusPersistencyFoConfiguration() : base(_jsonFiles)
        {
        }
    }
}
