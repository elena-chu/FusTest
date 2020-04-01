using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.UIServices;
using Prism.Mvvm;
using System.Windows.Media.Media3D;
using Ws.Fus.Interfaces.Calibration;
using Prism.Commands;

namespace WpfUI.Calibration.ViewModels
{
    // This is the declaration for binding to the view
    //[TypeDescriptionProvider(typeof(MethodBinding))]

    public class XDCalibViewModel : BindableBase
    {
        // C# interface
        private readonly ILocateXD _locateXDModel;

        //private PointRAS? _xdLoc;
        private Point3D? _xdLoc;

        public XDCalibViewModel(ILocateXD locateXDModel)
        {
            _locateXDModel = locateXDModel;
            _locateXDModel.CanLocateXDChanged += XDCalibViewModel_CanLocateXDChanged;
            _locateXDModel.XDLocChanged += XDCalibViewModel_XDLocChanged;

            LocateXD = new DelegateCommand(_locateXDModel.LocateXD, () => { return _locateXDModel.CanLocateXD; });
        }

        public DelegateCommand LocateXD { get; }

        public Point3D? XDLoc
        {
            get { return _xdLoc; }
            set { SetProperty(ref _xdLoc, value); }
        }
        private void XDCalibViewModel_CanLocateXDChanged(object sender, EventArgs ea)
        {
            LocateXD.RaiseCanExecuteChanged();    //.Refresh();  
        }
        private void XDCalibViewModel_XDLocChanged(object sender, EventArgs ea)
        {
            XDLoc = _locateXDModel.XDLoc;
        }
    }
}

