using Ws.Dicom.Persistency.Interfaces.Services;
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
using WpfUI.ViewModels;

namespace WpfUI.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();

            Loaded += MainView_Loaded;
		}

        private MainViewModel ViewModel => DataContext as MainViewModel;

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SwitchLayout();
            ViewModel.SeriesSelected += OnViewModelSeriesSelected;
        }

        private void OnViewModelSeriesSelected(object sender, EventArgs args)
        {
            bool closed = false;
            EventHandler onClosed = null;

            onClosed = (o, e) =>
            {
                SeriesSelector.Closed -= onClosed;
                closed = true;
            };

            SeriesSelector.Closed += onClosed;
            Dispatcher.Invoke(() => { SeriesSelector.IsOpen = false; });

            while (!closed)
                Task.Delay(1).Wait();
        }

        private void OnLoadSeriesClick(object sender, RoutedEventArgs e) => SeriesSelector.IsOpen = true;

        private void OnSliderClick(object sender, RoutedEventArgs e)
        { }
    }
}
