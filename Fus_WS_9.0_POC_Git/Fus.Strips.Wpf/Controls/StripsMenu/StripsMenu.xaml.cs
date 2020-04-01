using Extensions.Wpf.Patterns;
using Fus.Strips.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Fus.Strips.Wpf.Controls.StripsMenu
{
    /// <summary>
    /// Interaction logic for StripsMenu.xaml
    /// </summary>
    public partial class StripsMenu : UserControl
    {
        private DragNDropHelper _stripsDragNDrop;

        public static readonly DependencyProperty StripTemplateProperty =
            DependencyProperty.Register(nameof(StripTemplate), typeof(DataTemplate), typeof(StripsMenu), new PropertyMetadata(null as DataTemplate));

        public static readonly DependencyProperty Group1Property =
            DependencyProperty.Register(nameof(Group1), typeof(string), typeof(StripsMenu), new PropertyMetadata(null as string));

        public static readonly DependencyProperty Group2Property =
            DependencyProperty.Register(nameof(Group2), typeof(string), typeof(StripsMenu), new PropertyMetadata(null as string));

        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register(nameof(ImageSize), typeof(double), typeof(StripsMenu), new PropertyMetadata(100.0));

        public static readonly DependencyProperty ImageConverterProperty =
            DependencyProperty.Register(nameof(ImageConverter), typeof(IValueConverter), typeof(StripsMenu), new PropertyMetadata(new StripToImageConverter()));

        public static readonly DependencyProperty StripsProperty =
            DependencyProperty.Register(nameof(Strips), typeof(IEnumerable<IStrip>), typeof(StripsMenu), new PropertyMetadata(null));

        public StripsMenu()
        {
            InitializeComponent();
            Loaded += StripsMenu_Loaded;
        }

        public DataTemplate StripTemplate
        {
            get { return (DataTemplate)GetValue(StripTemplateProperty); }
            set { SetValue(StripTemplateProperty, value); }
        }

        public string Group1
        {
            get { return (string)GetValue(Group1Property); }
            set { SetValue(Group1Property, value); }
        }

        public string Group2
        {
            get { return (string)GetValue(Group2Property); }
            set { SetValue(Group2Property, value); }
        }

        public double ImageSize
        {
            get { return (double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        public IValueConverter ImageConverter
        {
            get { return (IValueConverter)GetValue(ImageConverterProperty); }
            set { SetValue(ImageConverterProperty, value); }
        }

        public IEnumerable<IStrip> Strips
        {
            get { return (IEnumerable<IStrip>)GetValue(StripsProperty); }
            set { SetValue(StripsProperty, value); }
        }

        private void StripsMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (StripTemplate != null)
                lvStrips.ItemTemplate = StripTemplate;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvStrips.ItemsSource);

            if (!string.IsNullOrWhiteSpace(Group1))
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription(Group1);
                view.GroupDescriptions.Add(groupDescription);
            }

            if (!string.IsNullOrWhiteSpace(Group2))
            {
                PropertyGroupDescription groupDescription2 = new PropertyGroupDescription(Group2);
                view.GroupDescriptions.Add(groupDescription2);
            }
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
