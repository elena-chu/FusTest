using Ws.Extensions.UI.Wpf.Controls;
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
using System.Windows.Threading;
using System.Collections.Specialized;

namespace Ws.Extensions.UI.Wpf.Patterns.DynamicGrid
{
    /// <summary>
    /// Interaction logic for DynamicGrid.xaml
    /// </summary>
    public partial class DynamicGrid : SizeToAspectRatio
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable<IDynamicGridItemLayout>), typeof(DynamicGrid), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DynamicGrid), new PropertyMetadata(null as DataTemplate));

        public DynamicGrid()
        {
            InitializeComponent();

            Loaded += DynamicGrid_Loaded;

            //((INotifyCollectionChanged)DynamicItems.Items).CollectionChanged += DynamicGrid_CollectionChanged;

            //DynamicItems.ItemsSource
        }

        private void DynamicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            VirtualizingStackPanel itemsPanel = GetVisualChild<VirtualizingStackPanel>(DynamicItems);
            if (itemsPanel != null && itemsPanel.IsItemsHost)
            {
                itemsPanel.PreviewMouseLeftButtonDown += delegate { MessageBox.Show("WPF"); };
            }
        }

        private void DynamicGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            return;
        }

        public IEnumerable<IDynamicGridItemLayout> Items
        {
            get { return (IEnumerable<IDynamicGridItemLayout>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static T GetVisualChild<T>(Visual referenceVisual) where T : Visual
        {
            Visual child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;
                if (child != null && (child.GetType() == typeof(T)))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetVisualChild<T>(child);
                    if (child != null && (child.GetType() == typeof(T)))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }
    }
}
