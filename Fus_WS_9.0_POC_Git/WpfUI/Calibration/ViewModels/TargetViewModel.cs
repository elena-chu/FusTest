using NirDobovizki.MvvmMonkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.ViewModels;
using Ws.Fus.Interfaces.Calibration;
using Ws.Fus.Interfaces.Coordinates;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Calibration.ViewModels
{
    // This is the declaration for binding to the view
    [TypeDescriptionProvider(typeof(MethodBinding))]

    // the view model implements INotifyPropertyChanged
    // the event is PropertyChanged
    public class TargetViewModel : ViewModelBase
    {
        // C# interface
        private readonly ITargetLocation _targetModel;
        private readonly ITransformation _transformationInterface;
        private readonly ICalibration _calibrationInterface;
        private readonly IACPC _acpcInterface;

        public TargetViewModel(ITargetLocation targetModel, ITransformation transformation, ICalibration calibration, IACPC acpc)
        {
            _targetModel = targetModel;
            _transformationInterface = transformation;
            _calibrationInterface = calibration;
            _acpcInterface = acpc;

            // subscribe to the event with the proper function
            // the event is TargetLocationChangedRas
            // the function when the event is fired is TargetModel_TargetLocationChangedRas
            _targetModel.TargetLocationChangedRas += TargetModel_TargetLocationChangedRas;
            _targetModel.TargetAcPcEnabledChanged += TargetModel_TargetAcPcEnabledChanged;
            _calibrationInterface.CalibrationChanged += CalibrationInterface_CalibrationChanged;
            _acpcInterface.ACPCLengthChanged += ACPCInterface_ACPCLengthChanged;
            _acpcInterface.ACPCOriginUpdated += ACPCInterface_ACPCOriginUpdated;

        }

        private void CalibrationInterface_CalibrationChanged(object sender, EventArgs ea)
        {
            UpdateXdToTarget();
        }

        private void ACPCInterface_ACPCLengthChanged(object sender, EventArgs ea)
        {
            var acpclength = _acpcInterface.GetACPCLength();
            if (acpclength.HasValue)
            {
                AcpcLength = acpclength;
            }

        }

        private void ACPCInterface_ACPCOriginUpdated(object sender, EventArgs ea)
        {
            var acpcorigin = _acpcInterface.GetCurrentAtlasOriginFactorString();
            if(acpcorigin!="")
            {
                AcpcOrigin = acpcorigin;
            }


        }

        // this is the function when the event is fired (fields availability has changed). 
        // The event Args contain the new enable/disable status of the fields
        private void TargetModel_TargetAcPcEnabledChanged(object sender, TargetAcPcEnabledChangedEventArgs ea)
        {
            TargetAcpcEnabled = ea.IsEnabled;
        }

        // this is the function when the event is fired (target location is changed). 
        // The event Args contain the new location of the target in RAS
        private void TargetModel_TargetLocationChangedRas(object sender, TargetLocationChangedRasEventArgs ea)
        {
            //TargetRASRL = ea.TargetLocationRas.RL;
            //TargetRASAP = ea.TargetLocationRas.AP;
            //TargetRASSI = ea.TargetLocationRas.SI;

            PointRAS? target = ea.TargetLocationRas;
            if (target.HasValue)
            {
                TargetRASRL = (float)target.Value.RL;
                TargetRASAP = (float)target.Value.AP;
                TargetRASSI = (float)target.Value.SI;
            }
            else
            {
                TargetRASRL = null;
                TargetRASAP = null;
                TargetRASSI = null;
            }
        }

        private void TargetACPCChanged()
        {
            if (TargetACPCML.HasValue && TargetACPCAP.HasValue && TargetACPCSI.HasValue)
            {

                var ras = _transformationInterface.TransformACPCToRAS(new PointAcPc(TargetACPCML.Value, TargetACPCAP.Value, TargetACPCSI.Value));
                if (ras.HasValue)
                {
                    _targetRasAp = ras.Value.AP;
                    _targetRasRl = ras.Value.RL;
                    _targetRasSi = ras.Value.SI;
                    Notify(nameof(TargetRASAP));
                    Notify(nameof(TargetRASRL));
                    Notify(nameof(TargetRASSI));
                }

            }
            UpdateXdToTarget();

        }

        private void TargetRasChanged()
        {
            if (TargetRASRL.HasValue && TargetRASAP.HasValue && TargetRASSI.HasValue)
            {

                PointRAS targetRAS = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);

                var acpc = _transformationInterface.TransformRASToACPC(targetRAS);
                if (acpc.HasValue)
                {
                    _targetAcpcAp = acpc.Value.AP;
                    _targetAcpcMl = acpc.Value.ML;
                    _targetAcpcSi = acpc.Value.SI;
                    Notify(nameof(TargetACPCAP));
                    Notify(nameof(TargetACPCML));
                    Notify(nameof(TargetACPCSI));
                }

            }
            UpdateXdToTarget();

        }

        private void UpdateXdToTarget()
        {
            XdToTargetRASRL = null;
            XdToTargetRASAP = null;
            XdToTargetRASSI = null;

            if (TargetRASRL.HasValue && TargetRASAP.HasValue && TargetRASSI.HasValue)
            {
                var targetRAS = new PointRAS(_targetRasRl.Value, _targetRasAp.Value, _targetRasSi.Value);

                PointRAS focalVsTarget = new PointRAS(0, 0, 0); ;
                var focalRAS = _calibrationInterface.GetNaturalFocusRAS();
                if (focalRAS.HasValue)
                {
                    focalVsTarget = targetRAS - focalRAS.Value;
                    XdToTargetRASRL = focalVsTarget.RL;
                    XdToTargetRASAP = focalVsTarget.AP;
                    XdToTargetRASSI = focalVsTarget.SI;

                }

            }

        }

        private double? _targetRasRl;
        // this is the property of the view model that binds to the view
        public double? TargetRASRL
        {
            get { return _targetRasRl; }
            set
            {
                if (value != _targetRasRl)
                {
                    _targetRasRl = value;
                    if (TargetRASRL.HasValue && TargetRASAP.HasValue && TargetRASSI.HasValue)
                    {
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                        TargetRasChanged();
                    }
                    // Notify the UI (view)
                    // In this case TargetView.xaml <xctk:DoubleUpDown Value="{Binding TargetRASRL}"
                    //even if the change is from UI, need to call Notify because we don't know that the trigger is from UI
                    Notify();
                }
            }
        }
        private double? _targetRasAp;
        public double? TargetRASAP
        {
            get { return _targetRasAp; }
            set
            {
                if (value != _targetRasAp)
                {
                    _targetRasAp = value;
                    if (TargetRASRL.HasValue && TargetRASAP.HasValue && TargetRASSI.HasValue)
                    {
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                        TargetRasChanged();
                    }
                    Notify();
                }
            }
        }
        private double? _targetRasSi;
        public double? TargetRASSI
        {
            get { return _targetRasSi; }
            set
            {
                if (value != _targetRasSi)
                {
                    _targetRasSi = value;
                    if (TargetRASRL.HasValue && TargetRASAP.HasValue && TargetRASSI.HasValue)
                    {
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                        TargetRasChanged();
                    }
                    Notify();
                }
            }
        }

        private bool _targetAcpcEnabled;
        public bool TargetAcpcEnabled
        {
            get { return _targetAcpcEnabled; }
            //Set - checks if the value has changes, if it changed, save the value 
            //and sends notification for data binding
            set { Set(ref _targetAcpcEnabled, value); }
        }


        private double? _targetAcpcMl;
        public double? TargetACPCML
        {
            get { return _targetAcpcMl; }
            set
            {
                if (value != _targetAcpcMl)
                {
                    _targetAcpcMl = value;
                    if (TargetACPCML.HasValue && TargetACPCAP.HasValue && _targetAcpcSi.HasValue)
                    {
                        TargetACPCChanged();
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                    }
                    Notify();
                }
            }
        }
        private double? _targetAcpcAp;
        public double? TargetACPCAP
        {
            get { return _targetAcpcAp; }
            set
            {
                if (value != _targetAcpcAp)
                {
                    _targetAcpcAp = value;
                    if (TargetACPCML.HasValue && TargetACPCAP.HasValue && _targetAcpcSi.HasValue)
                    {
                        TargetACPCChanged();
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                    }
                    Notify();
                }
            }
        }
        private double? _targetAcpcSi;
        public double? TargetACPCSI
        {
            get { return _targetAcpcSi; }
            set
            {
                if (value != _targetAcpcSi)
                {
                    _targetAcpcSi = value;
                    if (TargetACPCML.HasValue && TargetACPCAP.HasValue && _targetAcpcSi.HasValue)
                    {
                        TargetACPCChanged();
                        _targetModel.TargetLocationRas = new PointRAS(TargetRASRL.Value, TargetRASAP.Value, TargetRASSI.Value);
                    }
                    Notify();
                }
            }
        }

        private double? _xdToTargetRASRL;

        public double? XdToTargetRASRL
        {
            get { return _xdToTargetRASRL; }
            set
            {
                if (value != _xdToTargetRASRL)
                {
                    _xdToTargetRASRL = value;
                    Notify();
                }
            }
        }

        private double? _xdToTargetRASAP;

        public double? XdToTargetRASAP
        {
            get { return _xdToTargetRASAP; }
            set
            {
                if (value != _xdToTargetRASAP)
                {
                    _xdToTargetRASAP = value;
                    Notify();
                }
            }
        }

        private double? _xdToTargetRASSI;

        public double? XdToTargetRASSI
        {
            get { return _xdToTargetRASSI; }
            set
            {
                if (value != _xdToTargetRASSI)
                {
                    _xdToTargetRASSI = value;
                    Notify();
                }
            }
        }
        private float? _acpcLength;

        public float? AcpcLength
        {
            get { return _acpcLength; }
            set
            {
                    var acpclength = _acpcInterface.GetACPCLength();
                    if (acpclength.HasValue && acpclength != _acpcLength)
                    {
                        _acpcLength = acpclength;
                        Notify(nameof(AcpcLength));
                    }
                    

            }

        }

        private string _acpcOrigin;

        public string AcpcOrigin
        {
            get { return _acpcOrigin; }
            set
            {

                var acpcorigin = _acpcInterface.GetCurrentAtlasOriginFactorString();
                if (acpcorigin != _acpcOrigin)
                {
                    _acpcOrigin = acpcorigin;
                    Notify(nameof(AcpcOrigin));
                }

            }

        }
    }
}