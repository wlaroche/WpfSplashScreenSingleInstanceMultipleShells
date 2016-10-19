using SingleInstanceApp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using WpfSplashScreen.Infrastructure.Interfaces.Services;
using WpfSplashScreen.Services;

namespace WpfSplashScreen
{
    public partial class App : Application, ISingleInstance
    {
        //Set a Unique Identifier for the single Instance Process
        private const string _appUniqueId = "24F19F27-2589-4994-98A4-61AAEBFD6866";

        private static AppLoader _appLoader = null;
        private static bool _isFirstInstance = false;

        public ILoggingService Logger { get; set; }

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
            try
            {
                Logger = _appLoader.Logger;
            }
            catch (Exception)
            {
                Logger = LoggingService.Instance;
            }

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
                Logger.LogException(ex);
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
                Logger.LogException(e.Exception);
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

            if (_appLoader.IsAppInitialized == false)
                return true;

            _appLoader.RunApplication(false, args);
            return true;
        }
    }
}