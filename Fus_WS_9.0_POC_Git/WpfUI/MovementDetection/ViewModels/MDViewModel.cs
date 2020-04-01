using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.UIServices;
using Prism.Mvvm;
using System.Windows.Media.Media3D;
using Ws.Fus.Interfaces.MovementDetection;
using Prism.Commands;

namespace WpfUI.MovementDetection.ViewModels
{
    // This is the declaration for binding to the view
    //[TypeDescriptionProvider(typeof(MethodBinding))]

    public class MDViewModel : BindableBase
    {
        // C# interface
        private readonly IMovementDetection _mdModel;

        private Point3D? _mdVector;

        public MDViewModel(IMovementDetection mdModel)
        {
            _mdModel = mdModel;
            _mdModel.CanStartRefScanChanged += MDModel_CanStartRefScanChanged;
            _mdModel.CanDetectChanged += MDModel_CanDetectChanged;
            _mdModel.MDVectorChanged += MDModel_MDVectorChanged;

            StartRefScan = new DelegateCommand(_mdModel.StartRefScan, () => { return _mdModel.CanStartRefScan; });
            Detect = new DelegateCommand(_mdModel.Detect, () => { return _mdModel.CanDetect; });
        }

        public DelegateCommand Detect { get; }

        public Point3D? MDVector
        {
            get { return _mdVector; }
            set { SetProperty(ref _mdVector, value); }
        }

        public DelegateCommand StartRefScan { get; }
        private void MDModel_CanDetectChanged(object sender, EventArgs ea) //CanPerformMDChangedEventArgs e)
        {
            Detect.RaiseCanExecuteChanged();
        }

        private void MDModel_CanStartRefScanChanged(object sender, EventArgs ea)
        {
            StartRefScan.RaiseCanExecuteChanged();
        }
        private void MDModel_MDVectorChanged(object sender, EventArgs ea)
        {
            MDVector = _mdModel.MDVector;
        }
    }
}
