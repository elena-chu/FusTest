using Ws.Dicom.Persistency.UI.Wpf.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Persistency.UI.Wpf.ViewModels;
using Ws.Dicom.Persistency.Interfaces.Controllers;

namespace Ws.Dicom.Persistency.UI.Wpf.Module
{
    public class FusPersistencyWpfModule : IModule
    {
        //private IRegionManager _regionManager;
        //private IEventAggregator _eventAggregator;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("SeriesSelector", typeof(SeriesSelectorView));
            //_eventAggregator = containerProvider.Resolve<IEventAggregator>();

            //_eventAggregator.GetEvent<SelectSeriesEvent>().Subscribe(OnSelectSeriesRequest, true);
        }
        
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<SeriesSelectorView>();

            containerRegistry.RegisterSingleton<SeriesSelectorViewModel>();
            containerRegistry.RegisterSingleton<ISeriesSelectorController, SeriesSelectorViewModel>();
            return;
        }

        //private void OnSelectSeriesRequest(SelectSeriesRequest request)
        //{
        //    var view = new SeriesSelectorView();

        //    view.ViewModel.SelectSeriesRequest = request;
        //    request.Selected += OnSeriesSelected;

        //    var region = _regionManager.Regions["SeriesSelector"];
        //    region.RemoveAll();
        //    region.Add(view);
        //    region.Activate(view);
        //}

        //private void OnSeriesSelected(object sender, SelectSeriesResponse e)
        //{
        //    (sender as SelectSeriesRequest).Selected -= OnSeriesSelected;

        //    var region = _regionManager.Regions["SeriesSelector"];
        //    region.RemoveAll();

        //    _eventAggregator.GetEvent<SeriesSelectedEvent>().Publish(e);
        //}
    }
}
