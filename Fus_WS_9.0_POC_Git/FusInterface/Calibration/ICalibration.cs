using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;

namespace Ws.Fus.Interfaces.Calibration
{
    public interface ICalibration
    {
        PointRAS? GetNaturalFocusRAS();

        event EventHandler CalibrationChanged;
    }
}
