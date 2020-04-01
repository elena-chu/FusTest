using NirDobovizki.MvvmMonkey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Overlays.ViewModels;
using WpfUI.ViewModels;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Calibration.ViewModels
{
    [TypeDescriptionProvider(typeof(MethodBinding))]
	public class CalibrationControlViewModel : ViewModelBase
    {
        private readonly MainViewModel _uiModeModel;

        private bool _canAddFiducial;
        private bool _canAddTarget;
        private UiMode _currentMode;
        private IRigidNPR _rigidNPR;
        public ObservableCollection<RigidNPRTypesViewModel> AvailableRigidNPRTypes { get; private set; }

        public CalibrationControlViewModel(MainViewModel uiModeModel,IRigidNPR rigidNPR)
		{
			_uiModeModel = uiModeModel;
            _rigidNPR = rigidNPR;
			_uiModeModel.UiModeChanged += (_, ea) =>
			{
				_currentMode = ea.NewMode;
                //Notify(nameof(AddTargetSelected));
            };
			_uiModeModel.SubscribeOnCanEnterMode(UiMode.TargetOverlay, (_, ea) => CanAddTarget = ea.CanEnter);
            _uiModeModel.SubscribeOnCanEnterMode(UiMode.FiducialsOverlay, (_, ea) => CanAddFiducial = ea.CanEnter);

            AvailableRigidNPRTypes = new ObservableCollection<RigidNPRTypesViewModel>();
            AvailableRigidNPRTypes.Clear();
            AvailableRigidNPRTypes.AddRange(_rigidNPR.GetMenuItems().Select(o => new RigidNPRTypesViewModel(o.NPRRadius, o.Name)));

        }

        /*
                public void AddTarget()
                {
                    _uiModeModel.EnterUIMode(UiMode.SetTarget);
                }
                */
        public bool CanAddFiducial
        {
            get { return _canAddFiducial; }
            private set { Set(ref _canAddFiducial, value); }
        }

        public bool CanAddTarget
        {
			get { return _canAddTarget; }
			private set { Set(ref _canAddTarget, value); }
		}
/*
		private bool AddTargetSelected
		{
			get { return _currentMode == UiMode.SetTarget; }
		}



        public void AddFiducial()
        {
            _uiModeModel.EnterUIMode(UiMode.AddFiducials);
        }
*/
    }
}
