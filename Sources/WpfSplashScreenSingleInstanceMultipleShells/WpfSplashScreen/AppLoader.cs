using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using WpfSplashScreen.Helpers;
using WpfSplashScreen.Infrastructure.Interfaces;
using WpfSplashScreen.Models.Enums;
using WpfSplashScreen.Services;
using WpfSplashScreen.ViewModels;
using WpfSplashScreen.Views;

namespace WpfSplashScreen
{
    public class AppLoader : IDisposable
    {
        //-------------------- Fields --------------------
        private ILoggingService _loggingService;

        //-------------------- Properties --------------------
        public FileVersionInfo FileVersionInfo { get; private set; }

        public Container Container { get; private set; }
        public string ExecutableDirPath { get; private set; }
        public object DependencyResolver { get; private set; }

        //-------------------- Constructor --------------------
        public AppLoader()
        {
            FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            //Temporary set the logger
            _loggingService = LoggingService.Instance;
            _loggingService.Log("----------");
            _loggingService.Log("{0} {1} {2} start.", FileVersionInfo.CompanyName,
                                                      FileVersionInfo.ProductName,
                                                      FileVersionInfo.ProductVersion);
        }

        public void Initialize()
        {
            InitializeCulture();
            InitializeContainer();
        }

        public void Dispose()
        {
            _loggingService.Log("{0} {1} exit.", FileVersionInfo.CompanyName,
                                                      FileVersionInfo.ProductName,
                                                      FileVersionInfo.ProductVersion);
            Container.Dispose();
        }

        public bool CanClose()
        {
            return true;
        }

        private void InitializeCulture()
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalSeparator = ".";
            //English
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            _loggingService.Log("Culture initialized");
        }

        private void InitializeContainer()
        {
            // Create the container as usual.
            var container = new Container();

            // Register your types, for instance:
            container.Register<ILoggingService, LoggingService>(Lifestyle.Singleton);
            container.Register<IComputeService, ComputeService>(Lifestyle.Singleton);

            // Register your windows and view models:
            container.Register<IShellWindowViewModel, ShellWindowViewModel>();
            container.Register<IShellWindowView, ShellWindowView>();

            container.Register<ISplashScreenViewModel, SplashScreenViewModel>();
            container.Register<ISplashScreenView, SplashScreenView>();

            container.Verify(VerificationOption.VerifyOnly);
            Container = container;

            _loggingService.Log("Container initialized");
        }

        public void RunApplication(bool showSplashScreen, IList<string> args)
        {
            _loggingService.Log("Running Application");

            try
            {
                if (showSplashScreen == true)
                {
                    var app = new App();

                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this, EToken.MESSAGEBOX, true, (x) =>
                        {
                            Utils.InvokeOnUiThread(() =>
                            {
                                MessageBox.Show(x);
                            });
                        });

                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<IList<string>>(this, EToken.NEWSHELL, true, (arguments) =>
                    {
                        Utils.InvokeOnUiThread(() =>
                        {
                            GenerateNewShellWindow(arguments);
                        });
                    });

                    var splash = Container.GetInstance<ISplashScreenView>();
                    splash.ViewModel.OnLoadCompeted += (s, evt) =>
                    {
                        var shellWindow = Container.GetInstance<IShellWindowView>();
                        shellWindow.Initialize();
                        (shellWindow as Window).Show();

                        (splash as Window).Close();

                        if (args != null && args.Count > 0)
                            shellWindow.ViewModel.ProcessArgs(args);
                    };
                    splash.ViewModel.OnLoadFailed += (s, evt) =>
                    {
                        (splash as Window).Close();
                    };

                    (splash as Window).Show();
                    splash.Initialize();
                    app.Run((Window)splash);
                }
                else
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<IList<string>>(null, EToken.NEWSHELL);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorException(ex, "Error during RunApplication");
            }
        }

        private void GenerateNewShellWindow(IList<string> args = null)
        {
            _loggingService.Log("Creating new application instance");
            var shellWindow = Container.GetInstance<IShellWindowView>();
            shellWindow.Initialize();
            (shellWindow as Window).Show();

            if (args != null && args.Count > 0)
                shellWindow.ViewModel.ProcessArgs(args);
        }
    }
}