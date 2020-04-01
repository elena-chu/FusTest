using Ws.Fus.UI.Wpf.Entities;
using Ws.Fus.UI.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ws.Fus.UI.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for FusPoint3DControl.xaml
    /// </summary>
    public partial class FusPoint3DControl : UserControl
    {
        public static readonly DependencyProperty Point3DProperty = DependencyProperty.Register(
            nameof(Point3D),
            typeof(Point3D?),
            typeof(FusPoint3DControl),
            new FrameworkPropertyMetadata(default(Point3D?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Point3D_DependencyPropertyChanged)));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            nameof(Mode),
            typeof(FusPoint3DMode),
            typeof(FusPoint3DControl),
            new PropertyMetadata(FusPoint3DMode.Xyz, new PropertyChangedCallback(Mode_DependencyPropertyChanged)));

        public string LabelX { get; set; } = "X";
        public string LabelY { get; set; } = "Y";
        public string LabelZ { get; set; } = "Z";


        private bool _listen = true;

        public FusPoint3DControl()
        {
            InitializeComponent();

            XYZ.PropertyChanged += BindableXYZ_PropertyChanged;
        }

        public BindableXyzVM XYZ { get; } = new BindableXyzVM();

        public Point3D? Point3D
        {
            get { return (Point3D?)GetValue(Point3DProperty); }
            set
            {
                SetValue(Point3DProperty, value);
            }
        }

        public FusPoint3DMode Mode
        {
            get { return (FusPoint3DMode)GetValue(ModeProperty); }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        private static void Point3D_DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (d as FusPoint3DControl);
            if (!self._listen)
                return;

            var bindableXYZ = self.XYZ;
            var newValue = (Point3D?)e.NewValue;

            bindableXYZ.X = newValue?.X;
            bindableXYZ.Y = newValue?.Y;
            bindableXYZ.Z = newValue?.Z;
        }

        private static void Mode_DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (d as FusPoint3DControl);
            var newValue = (FusPoint3DMode)e.NewValue;
            self.Mode = newValue;
        }

        private void BindableXYZ_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _listen = false;

            if (XYZ.X.HasValue && XYZ.Y.HasValue && XYZ.Z.HasValue)
                Point3D = new Point3D(XYZ.X.Value, XYZ.Y.Value, XYZ.Z.Value);
            else
                Point3D = null;

            _listen = true;
        }
    }
}
