using Ws.Dicom.Persistency.UI.Wpf.ViewModels;
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
using Ws.Extensions.AppSettings.Patterns;
using Ws.Extensions.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using Ws.Dicom.Interfaces.Entities;
using Ws.Extensions.UI.Wpf.Patterns;

namespace Ws.Dicom.Persistency.UI.Wpf.Views
{
    /// <summary>
    /// Interaction logic for StudySelectorView.xaml
    /// </summary>
    public partial class SeriesSelectorView : UserControl
    {
        private const string SeriesToAdd = nameof(SeriesToAdd);
        private const string SeriesToRemove = nameof(SeriesToRemove);

        private SeriesSelectorViewModel _viewModel;

        private DragNDropHelper _studyDetailsDragNDrop;
        private DragNDropHelper _selectedSeriesDragNDrop;

        

        public SeriesSelectorView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                AppSettingsClassAttribute.Load(DataContext);
            };

            Dispatcher.ShutdownStarted += (s, e) =>
            {
                AppSettingsClassAttribute.Save(DataContext);
            };
        }

        internal SeriesSelectorViewModel ViewModel => DataContext as SeriesSelectorViewModel;


        private void StudyDetails_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListView))
                return;

            _studyDetailsDragNDrop = new DragNDropHelper(sender, e, SeriesToAdd, CreateStudyDetailsDnDData);
        }

        private void SelectedSeries_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedSeriesDragNDrop = new DragNDropHelper(sender, e, SeriesToRemove, CreateSelectedSeriesDnDData);
        }

        private DragNDropHelper.DragNDropData CreateStudyDetailsDnDData(object sender, MouseEventArgs e)
        {
            // Get the dragged ListViewItem
            ListView listView = sender as ListView;

            var series = (SeriesVm)listView.SelectedItem;
            if (series == null)
                return null;

            var effect = ViewModel.SelectedSeries.Contains(series) ? DragDropEffects.None : DragDropEffects.Copy;

            var listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);

            var pos = e.GetPosition(listViewItem);
            var rect = VisualTreeHelper.GetDescendantBounds(listViewItem);
            if (!rect.Contains(pos))
                return null;

            return new DragNDropHelper.DragNDropData
            {
                Data = series,
                DragSource = listViewItem,
                Effects = effect
            };
        }

        private void StudyDetails_PreviewMouseMove(object sender, MouseEventArgs e) => _studyDetailsDragNDrop?.PreviewMouseMove(sender, e);
        

        protected void StudyDetails_SeriesDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListViewItem))
                return;

            var listViewItem = (ListViewItem)sender;
            var series = (SeriesVm)listViewItem.DataContext;
            if (!ViewModel.SelectedSeries.Contains(series))
                ViewModel.SelectedSeries.Add(series);
        }

        private DragNDropHelper.DragNDropData CreateSelectedSeriesDnDData(object sender, MouseEventArgs e)
        {
            // Get the dragged ListViewItem
            ListView listView = sender as ListView;

            var series = (SeriesVm)listView.SelectedItem;
            if (series == null)
                return null;

            var listViewItem = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);

            return new DragNDropHelper.DragNDropData
            {
                Data = series,
                DragSource = listViewItem,
                Effects = DragDropEffects.Move
            };
        }

        private void SelectedSeries_PreviewMouseMove(object sender, MouseEventArgs e) => _selectedSeriesDragNDrop?.PreviewMouseMove(sender, e);
        

        private void SelectedSeries_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(SeriesToAdd))
            {
                var series = e.Data.GetData(SeriesToAdd) as SeriesVm;
                ViewModel.SelectedSeries.Add(series);
            }
        }

        private void SelectedSeries_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(SeriesToAdd) || sender == e.Source)
                e.Effects = DragDropEffects.None;
        }

        private void StudySelector_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(SeriesToRemove))
            {
                var pos = e.GetPosition(selectedSeriesPanel);
                var rect = VisualTreeHelper.GetDescendantBounds(selectedSeriesPanel);

                if (rect.Contains(pos))
                    return;

                var series = e.Data.GetData(SeriesToRemove) as SeriesVm;
                ViewModel.SelectedSeries.Remove(series);
            }
        }
    }
}
