using Prism.Ioc;
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
    /// Actions Builder for Measure menu
    public class MeasureActionBuilder
    {
        public const string ACPC90 = "ACPC90";

        private readonly IUiModeChanges _uiModeService;
        private readonly IRigidNPR _rigidNPRService;
        private readonly Func<MeasureActionViewModel> _measureFactory;
        private readonly Func<DeleteOverlayActionViewModel> _deleteFactory;

        public MeasureActionBuilder(IUiModeChanges uiModeService,
            IRigidNPR rigidNPRService,
            Func<MeasureActionViewModel> measureFactory,
            Func<DeleteOverlayActionViewModel> deleteFactory)
        {
            _uiModeService = uiModeService;
            _rigidNPRService = rigidNPRService;
            _measureFactory = measureFactory;
            _deleteFactory = deleteFactory;
        }

        public MeasureActionViewModel Build(UiMode uiMode, string subMode = null)
        {
            MeasureActionViewModel vm = _measureFactory();
            switch (uiMode)
            {
                case UiMode.RegionsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.RegionsOverlay, "Region"));
                    break;
                case UiMode.FiducialsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.FiducialsOverlay, "Fiducials"));
                    break;
                case UiMode.NPRPolygonsOverlay:
                    vm.Initialize(CreateActionParam(UiMode.NPRPolygonsOverlay, "NPR Polygon"));
                    break;
                case UiMode.RigidNPROverlay:
                    var nprRegionsParam = CreateActionParam(UiMode.RigidNPROverlay, "No Pass Regions(NPR)", hasSubAction: true);
                    nprRegionsParam.ChildActions = _rigidNPRService.GetMenuItems()
                        .Select(el => {
                            var param = CreateActionParam(UiMode.RigidNPROverlay, el.Name, el.NPRRadius);
                            return param;
                        })
                        .Cast<ActionInitializeParams>()
                        .ToList(); ;
                    vm.Initialize(nprRegionsParam);
                    vm.ChildActions.ForEach(el => el.NodeType = NodeType.ChildExecuteAlways);
                    break;
                case UiMode.TargetOverlay:
                    vm.Initialize(CreateActionParam(UiMode.TargetOverlay, "Target"));
                    break;
                case UiMode.MeasurementDistanceOverlay:
                    vm.Initialize(CreateActionParam(UiMode.MeasurementDistanceOverlay, "Distance"));
                    break;
                case UiMode.MeasurementAreaOverlay:
                    vm.Initialize(CreateActionParam(UiMode.MeasurementAreaOverlay, "Area"));
                    break;
                case UiMode.MeasurementAngleOverlay:
                    if(!string.IsNullOrWhiteSpace(subMode) && subMode == ACPC90)
                    {
                        vm.Initialize(CreateActionParam(UiMode.MeasurementAngleOverlay, "Angle 90 on AC/PC", ACPC90));
                        break;
                    }
                    var genericAngleParam = CreateActionParam(UiMode.MeasurementAngleOverlay, "Angle", hasSubAction: true);
                    var angleParam = CreateActionParam(UiMode.MeasurementAngleOverlay, "Angle", "0");
                    genericAngleParam.ChildActions.Add(angleParam);
                    var angle90Param = CreateActionParam(UiMode.MeasurementAngleOverlay, "Angle 90", "90");
                    genericAngleParam.ChildActions.Add(angle90Param);
                    vm.Initialize(genericAngleParam);
                    vm.ChildActions.ForEach(el => el.NodeType = NodeType.ChildExecuteAlways);
                    break;
                case UiMode.ACPCOverlay:
                    var acPcParam = CreateActionParam(UiMode.ACPCOverlay, "AC/PC", hasSubAction: true);
                    var acParam = CreateActionParam(UiMode.ACPCOverlay, "AC", ((int)(UiMode.ACOverlay)).ToString());
                    acPcParam.ChildActions.Add(acParam);
                    var pcParam = CreateActionParam(UiMode.ACPCOverlay, "PC", ((int)(UiMode.PCOverlay)).ToString());
                    acPcParam.ChildActions.Add(pcParam);
                    var midLineParam = CreateActionParam(UiMode.ACPCOverlay, "MidLine", ((int)(UiMode.MidLineOverlay)).ToString());
                    acPcParam.ChildActions.Add(midLineParam);
                    vm.Initialize(acPcParam);
                    vm.ChildActions.ForEach(el => el.NodeType = NodeType.ChildExecuteAlways);
                    break;
                default:
                    throw new NotSupportedException($"The {uiMode} doesn't belong to Measure type");
            }

            return vm;
        }

        public DeleteOverlayActionViewModel BuildDelete(bool isDeleteAll)
        {
            DeleteOverlayActionViewModel vm = _deleteFactory();
            vm.IsDeleteAll = isDeleteAll;

            string name = isDeleteAll ? "Delete All" : "Delete";
            vm.Initialize(new ActionInitializeParams() { Name = name});

            return vm;
        }

        public LayerActionInitializeParams CreateActionParam(UiMode layer, string name, string subUiMode = null, bool hasSubAction = false, NodeType nodeType = NodeType.None)
        {
            return new LayerActionInitializeParams
            {
                Name = name,
                Layer = layer,
                SubUiMode = subUiMode,
                ChildActions = hasSubAction ? new List<ActionInitializeParams>() : null,
                NodeType = nodeType,
            };
        }
    }
}
