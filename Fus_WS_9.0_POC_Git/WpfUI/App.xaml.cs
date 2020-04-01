using Ws.Dicom.Persistency.Fo.Module;
using Ws.Dicom.Persistency.UI.Wpf.Module;
using NirDobovizki.MvvmMonkey;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.Messages.ViewModels;
using WpfUI.Module;
using WpfUI.ViewModels;
using Ws.Fus.DicomViewer.UI.Wpf.Module;
using Ws.Fus.Fge.Interfaces;
using Ws.Fus.Fge.Module;
using Ws.Fus.Interfaces.Messages;
using Ws.Fus.Strips.Module;

namespace WpfUI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	//public partial class App : Application
	public partial class App : PrismApplication
    {
		private static FusApplicationInterface _fusInterface;

        public static FusApplicationInterface Fus => _fusInterface;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
		{
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;

			ViewLocator.RegisterViews(Resources, typeof(App).Assembly);

			_fusInterface = new FusApplicationInterface();
			_fusInterface.Start();

			Dispatcher.Hooks.DispatcherInactive += (_, __) => _fusInterface.CallIdle();
			base.OnStartup(e);

			// prepare for messages
			_fusInterface.GetGenericMessageInterface().MessageRequested += App_MessageRequested;

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                //.MinimumLevel.Information()
                //.MinimumLevel.Override("Dicom", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: "[{Timestamp} {Level:u3}] [{SourceContext}] [tid:{ThreadId}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(App.Current.Dispatcher);

            containerRegistry.RegisterInstance(_fusInterface.GetTargetLocationInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetUiModeChangesInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetTransformationInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetStripDataInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetCalibrationInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetMovementDetectionInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetXDLocationInterface());
            //containerRegistry.RegisterInstance(_fusInterface.GetStripsManager());
            containerRegistry.RegisterInstance(_fusInterface.GetACPCInterface());
            containerRegistry.RegisterInstance(_fusInterface.GetRigidNPRInterface());
            containerRegistry.RegisterSingleton<MainViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //moduleCatalog.AddModule<Fus.Events.FusEventsModule>();
            //moduleCatalog.AddModule<Fus.Adapters.FusAdaptersModule>();
            //moduleCatalog.AddModule<FusAppModule>();

            moduleCatalog.AddModule<FusPersistencyFoModule>();
            moduleCatalog.AddModule<FusPersistencyWpfModule>();
            moduleCatalog.AddModule<FusStripsModule>();
            moduleCatalog.AddModule<FgeModule>();
            moduleCatalog.AddModule<DicomViewerModule>();
            moduleCatalog.AddModule<WpfUiModule>();
            return;
        }

        private void App_MessageRequested(object sender, MessageRequestedEventArgs ea)
		{
			var vm = new GenericMessageViewModel();
			vm.ActionChecked = ea.ActionChecked;
			vm.ActionText = ea.ActionText;
			vm.HasAction = ea.HasAction;
			vm.MessageId = ea.MessageId;
			vm.MessageText = ea.MessageText;
			vm.MessageType = ea.MessageType;
			ea.Buttons.Select(o => new GenericMessageButton { ButtonText = string.IsNullOrWhiteSpace(o.Text)?null:o.Text, ButtonTip = string.IsNullOrWhiteSpace(o.Tip) ? null :o.Tip, ButtonResult = o.Result }).ToList().ForEach(o => vm.Buttons.Add(o));

			var wnd = new Window();
			wnd.SizeToContent = SizeToContent.WidthAndHeight;
			wnd.ResizeMode = ResizeMode.NoResize;
			wnd.WindowStyle = WindowStyle.None;
			wnd.Content = vm;
			wnd.Owner = Application.Current.MainWindow;
			wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			vm.CloseWindow = () => wnd.Close();
			wnd.ShowDialog();

			ea.ActionChecked = vm.ActionChecked;
			ea.SelectedButtonResult = vm.SelectedButtonResult;
		}

		private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.ToString());
		}

	}
}
