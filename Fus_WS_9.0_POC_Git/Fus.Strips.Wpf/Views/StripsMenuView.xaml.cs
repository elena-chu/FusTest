using Extensions.Wpf.Patterns;
using Fus.Strips.Contracts.Entities;
using Fus.Strips.Wpf.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fus.Strips.Wpf.Views
{
    /// <summary>
    /// Interaction logic for StripsMenuView.xaml
    /// </summary>
    public partial class StripsMenuView : UserControl
    {
        private DragNDropHelper _stripsDragNDrop;
        
        public StripsMenuView()
        {
            InitializeComponent();
        }

        private void Strips_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _stripsDragNDrop = new DragNDropHelper(sender, e, "StripDnD", CreateStripDnDData);
        }

        private void Strips_PreviewMouseMove(object sender, MouseEventArgs e) => _stripsDragNDrop?.PreviewMouseMove(sender, e);

        private DragNDropHelper.DragNDropData CreateStripDnDData(object sender, MouseEventArgs e)
        {
            // Get the dragged ListViewItem
            ListView listView = sender as ListView;

            var strip = (IStrip)listView.SelectedItem;
            if (strip == null)
                return null;

            var listViewItem = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);

            return new DragNDropHelper.DragNDropData
            {
                Data = strip,
                DragSource = listViewItem,
                Effects = DragDropEffects.Copy
            };
        }
    }
}
