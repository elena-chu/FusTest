using NirDobovizki.MvvmMonkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.ViewModels;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Overlays.ViewModels
{
	[TypeDescriptionProvider(typeof(MethodBinding))]
	public class OverlayShowHideViewModel : ViewModelBase
	{

		private MainViewModel _uiModeModel;
		//private UiMode _currentMode;

		public OverlayShowHideViewModel(MainViewModel uiModeModel)
		{
			_uiModeModel = uiModeModel;
            /*
			_uiModeModel.UiModeChanged += (_, ea) =>
			{
				_currentMode = ea.NewMode;
				Notify(nameof(AddTargetSelected));
			};
			_uiModeModel.SubscribeOnCanEnterMode(UiMode.SetTarget, (_, ea) => CanAddTarget = ea.CanEnter);*/

            NprEnabled = true;
            _uiModeModel.SubscribeOnCanShowHideLayer(UiMode.NPRPolygonsOverlay, (_, ea) => NprEnabled = ea.CanShowHide);
 
            NprVisible = true;
            _uiModeModel.SubscribeOnLayerVisiblityChanged(UiMode.NPRPolygonsOverlay, (_, ea) => NprVisible = uiModeModel.IsLayerVisible(UiMode.NPRPolygonsOverlay));

        }

        private bool _nprVisible;

        public bool NprVisible
        {
            get { return _nprVisible; }
            set
            {
                _nprVisible = value;
                Notify();

                if(_nprVisible )
                {
                    _uiModeModel.ShowLayer(UiMode.MeshNPRAirOverlay);
                    _uiModeModel.ShowLayer(UiMode.MeshNPROverlay);
                    _uiModeModel.ShowLayer(UiMode.NPRPolygonsOverlay);
                    _uiModeModel.ShowLayer(UiMode.RigidNPROverlay);
                }
                else
                {
                    _uiModeModel.HideLayer(UiMode.MeshNPRAirOverlay);
                    _uiModeModel.HideLayer(UiMode.MeshNPROverlay);
                    _uiModeModel.HideLayer(UiMode.NPRPolygonsOverlay);
                    _uiModeModel.HideLayer(UiMode.RigidNPROverlay);
                }
            }
        }

        private bool _nprEnabled;
        public bool NprEnabled
        {
            get { return _nprEnabled; }
            set { Set(ref _nprEnabled, value); }
        }



/*
		public void AddTarget()
		{
			_uiModeModel.EnterUIMode(UiMode.SetTarget);
		}

		private bool _canAddTarget;
		public bool CanAddTarget
		{
			get { return _canAddTarget; }
			private set { Set(ref _canAddTarget, value); }
		}

		private bool AddTargetSelected
		{
			get { return _currentMode == UiMode.SetTarget; }
		}


        */
	}
}
