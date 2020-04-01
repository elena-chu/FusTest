using Dicom.Imaging;
using Dicom.Log;
using Ws.Extensions.Patterns;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.Fo.Configuration;
using Ws.Dicom.Persistency.Fo.Services;
using Ws.Dicom.Persistency.Fo.Settings;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Module
{
    public class FusPersistencyFoModule : IModule
    {
        public FusPersistencyFoModule()
        {
            return;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var logger = Log.ForContext("SourceContext", "Dicom");
            LogManager.SetImplementation(new SerilogManager(logger));
            ImageManager.SetImplementation(WPFImageManager.Instance);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<Wrapper<FusPersistencyFoSettings>, FusPersistencyFoConfiguration>();
            containerRegistry.RegisterSingleton<ISearchServiceFactory, SearchServiceFactory>();
        }
    }
}
