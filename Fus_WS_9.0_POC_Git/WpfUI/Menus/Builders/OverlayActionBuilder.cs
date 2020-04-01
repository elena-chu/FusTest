using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Enums;
using WpfUI.Menus.Models;
using WpfUI.Menus.ViewModels;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.Builders
{
    /// <summary>
    /// Actions Builder for Overlay menu
    /// </summary>
    public class OverlayActionBuilder
    {
        public const string ANNOTATION = "Annotation";

        private readonly Func<OverlayActionViewModel> _overlayFactory;

        public OverlayActionBuilder(Func<OverlayActionViewModel> overlayFactory)
        {
            _overlayFactory = overlayFactory;
        }

        public OverlayActionViewModel Build(UiMode uiMode, string subMode = null)
        {
            OverlayActionViewModel vm = _overlayFactory();
            switch(uiMode)
            {
                case UiMode.None: //Temp till will be clear what to do
                    if(subMode == ANNOTATION) //Temp
                    {
                        var annotationParam = CreateActionParam(UiMode.TextOverlay, "Annotation", hasSubAction: true);
                        // List of Sub Actions should be received from FUS and converted to ActionInitializeParams
                        var userAnnotationParam = CreateActionParam(UiMode.TextOverlay, "User Annotation");
                        annotationParam.ChildActions.Add(userAnnotationParam);
                        var patientDetailsParam = CreateActionParam(UiMode.TextOverlay, "Patient Details");
                        annotationParam.ChildActions.Add(patientDetailsParam);
                        var imageInfoParam = CreateActionParam(UiMode.TextOverlay, "Image Information");
                        annotationParam.ChildActions.Add(imageInfoParam);
                        vm.Initialize(annotationParam);
                        vm.NodeType = NodeType.ContainerParentType;
                    }
                    break;
                case UiMode.TextOverlay:
                    break;
                case UiMode.TargetOverlay:
                    vm.Initialize(CreateActionParam(UiMode.TargetOverlay, "Target"));
                    break;
                case UiMode.FiducialsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.FiducialsOverlay, "Fiducials"));
                    break;
                case UiMode.MeasurementOverlay:
                    vm.Initialize(CreateActionParam(UiMode.MeasurementOverlay, "Measurements"));
                    break;
                case UiMode.RegionsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.RegionsOverlay, "Region"));
                    break;
                case UiMode.BathLimitsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.BathLimitsOverlay, "Bath Limits"));
                    break;
                case UiMode.CalibrationDataOverlay:
                    vm.Initialize(CreateActionParam(UiMode.CalibrationDataOverlay, "Transducer"));
                    break;
                case UiMode.PreOpSegmentationOverlay:
                    vm.Initialize(CreateActionParam(UiMode.PreOpSegmentationOverlay, "CT Mask"));
                    break;
                case UiMode.ScanGridOverlay:
                    vm.Initialize(CreateActionParam(UiMode.ScanGridOverlay, "Scan Grid"));
                    break;
                case UiMode.NPRPolygonsOverlay:
                    var nprRegionsParam = CreateActionParam(UiMode.NPRPolygonsOverlay, "No Pass Regions(NPR)", hasSubAction: true);
                    var meshNPRAirOverlayParam = CreateActionParam(UiMode.MeshNPRAirOverlay, "Mesh NPR Air");
                    nprRegionsParam.ChildActions.Add(meshNPRAirOverlayParam);
                    var meshNPROverlayParam = CreateActionParam(UiMode.MeshNPROverlay, "Mesh NPR");
                    nprRegionsParam.ChildActions.Add(meshNPROverlayParam);
                    var nprPolygonsOverlayParam = CreateActionParam(UiMode.NPRPolygonsOverlay, "NPR Polygons");
                    nprRegionsParam.ChildActions.Add(nprPolygonsOverlayParam);
                    var rigidNPROverlayParam = CreateActionParam(UiMode.RigidNPROverlay, "Rigid NPR");
                    nprRegionsParam.ChildActions.Add(nprPolygonsOverlayParam);
                    vm.Initialize(nprRegionsParam);
                    vm.NodeType = NodeType.ProxyContainerParentType;
                    break;
                case UiMode.ACPCOverlay:
                    var acPcParam = CreateActionParam(UiMode.ACPCOverlay, "AC/PC", hasSubAction: true);
                    var acParam = CreateActionParam(UiMode.ACOverlay, "AC");
                    acPcParam.ChildActions.Add(acParam);
                    var pcParam = CreateActionParam(UiMode.PCOverlay, "PC");
                    acPcParam.ChildActions.Add(pcParam);
                    var midLineParam = CreateActionParam(UiMode.MidLineOverlay, "MidLine");
                    acPcParam.ChildActions.Add(midLineParam);
                    vm.Initialize(acPcParam);
                    vm.NodeType = NodeType.ProxyContainerParentType;
                    break;
                default:
                    throw new NotSupportedException($"The {uiMode} not belongs to Overlay type");
            }

            return vm;
        }

        public LayerActionInitializeParams CreateActionParam(UiMode layer, string name, bool hasSubAction = false)
        {
            return new LayerActionInitializeParams
            {
                Name = name,
                Layer = layer,
                ChildActions = hasSubAction ? new List<ActionInitializeParams>() : null,
            };
        }
    }
}
