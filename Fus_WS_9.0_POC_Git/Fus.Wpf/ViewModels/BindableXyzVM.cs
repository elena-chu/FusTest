using Ws.Fus.UI.Wpf.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ws.Fus.UI.Wpf.ViewModels
{
    public class BindableXyzVM : BindableBase
    {
        private double? _x;
        private double? _y;
        private double? _z;

        public double? X
        {
            get { return _x; }
            set
            {
                SetProperty(ref _x, value);
            }
        }

        public double? Y
        {
            get { return _y; }
            set
            {
                SetProperty(ref _y, value);
            }
        }

        public double? Z
        {
            get { return _z; }
            set
            {
                SetProperty(ref _z, value);
            }
        }
    }
}
