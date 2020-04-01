using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public enum UiMode
	{
		None = 76,
        TextOverlay = 2,
        BathLimitsOverlay = 12,
        RegionsOverlay = 14,
        FiducialsOverlay = 21,
        CalibrationDataOverlay = 25,
        NPRPolygonsOverlay = 41,
        ScanGridOverlay = 44,
        PreOpSegmentationOverlay = 47,
        RigidNPROverlay = 57,
        MeshNPROverlay = 58,
        ACPCOverlay = 64,
        ACOverlay = 65,
        PCOverlay = 66,
        MidLineOverlay = 67,
        TargetOverlay = 68,
        MeasurementOverlay = 71,
        MeasurementDistanceOverlay = 72,
        MeasurementAreaOverlay = 73,
        MeasurementAngleOverlay = 74,
        MeshNPRAirOverlay = 75,

        Zoom = 80,
        Pan = 81,
        Windowing = 82,
    }
}
