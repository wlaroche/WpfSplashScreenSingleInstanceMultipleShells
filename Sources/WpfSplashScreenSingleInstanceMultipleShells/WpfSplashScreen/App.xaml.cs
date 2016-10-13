using SimpleInjector;
using SingleInstanceApp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using WpfSplashScreen.Infrastructure.Interfaces;

namespace WpfSplashScreen
{
    public partial class App : Application, ISingleInstance
    {
        private const string _appUniqueId = "24F19F27-2589-4994-98A4-61AAEBFD6866";

        private static AppLoader _appLoader = null;
        private static bool _isFirstInstance = false;

        public Container _container { get; set; }
        public ILoggingService _loggingService { get; set; }

        [STAThread]
        private static void Main(string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(_appUniqueId))
            {
                _isFirstInstance = true;

                _appLoader = new AppLoader();
                _appLoader.Initialize();

                _appLoader.RunApplication(true, args);

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
            else
            { }
        }

        public App()
        {
            this.InitializeComponent();

            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appLoader.Dispose();
            base.OnExit(e);
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null)
                return;
            try
            {
                _loggingService.LogException(ex);
                MessageBox.Show("", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            { }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            try
            {
                _loggingService.LogException(e.Exception);
                MessageBox.Show("", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            { }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            //Launch a ShellWindow in single instance app

            if (args != null && args.Count > 0)
            {
                args.RemoveAt(0);
            }

            _appLoader.RunApplication(false, args);
            return true;
        }
    }
}