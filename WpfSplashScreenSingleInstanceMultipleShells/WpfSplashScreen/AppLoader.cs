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
using WpfSplashScreen.Infrastructure.Interfaces.Services;
using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;
using WpfSplashScreen.Infrastructure.Interfaces.Views;
using WpfSplashScreen.Models.Enums;
using WpfSplashScreen.Services;
using WpfSplashScreen.ViewModels;
using WpfSplashScreen.Views;

namespace WpfSplashScreen
{
    public class AppLoader : IAppLoader, IDisposable
    {
        private App _currentApp = null;

        #region Constructor

        public bool IsAppInitialized { get; private set; }
        public bool IsAppAlreadyRun { get; private set; }

        public AppLoader()
        {
            FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            //Temporary set the logger
            Logger = LoggingService.Instance;
            Logger.Log("----------");
            Logger.Log("{0} {1} {2} start.", FileVersionInfo.CompanyName,
                                                      FileVersionInfo.ProductName,
                                                      FileVersionInfo.ProductVersion);
        }

        #endregion Constructor

        #region IDisposable Implementation

        public void Dispose()
        {
            Logger.Log("{0} {1} exit.", FileVersionInfo.CompanyName,
                                                      FileVersionInfo.ProductName,
                                                      FileVersionInfo.ProductVersion);
            Container.Dispose();
        }

        #endregion IDisposable Implementation

        private void InitializeCulture()
        {
            CultureInfo ci = new CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalSeparator = ".";
            //English
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            Logger.Log("Culture initialized");
        }

        private void InitializeContainer()
        {
            // Create the container as usual.
            var container = new Container();

            // Register this isntance in singleton in container
            container.Register<IAppLoader>(() => this, Lifestyle.Singleton);

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

            Logger.Log("Container initialized");
        }

        private void GenerateNewShellWindow(IList<string> args = null)
        {
            Logger.Log("Creating new application instance");
            var shellWindow = Container.GetInstance<IShellWindowView>();
            shellWindow.Initialize();
            (shellWindow as Window).Show();

            if (args != null && args.Count > 0)
                shellWindow.ViewModel.ProcessArgs(args);
        }

        #region IAppLoader Implementation

        public ILoggingService Logger { get; private set; }

        public FileVersionInfo FileVersionInfo { get; private set; }

        public Container Container { get; private set; }

        public string ExecutableDirPath { get; private set; }

        public object DependencyResolver { get; private set; }

        public void Initialize()
        {
            InitializeCulture();
            InitializeContainer();
            InitializeApp();
        }

        public void InitializeApp()
        {
            if (_currentApp == null)
                _currentApp = new App();
        }

        public void RunApplication(bool showSplashScreen, IList<string> args)
        {
            Logger.Log("Running Application");

            try
            {
                if (showSplashScreen == true)
                {
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

                        IsAppInitialized = true;

                        if (args != null && args.Count > 0)
                            shellWindow.ViewModel.ProcessArgs(args);
                    };
                    splash.ViewModel.OnLoadFailed += (s, evt) =>
                    {
                        (splash as Window).Close();
                    };

                    (splash as Window).Show();
                    splash.Initialize();

                    if (IsAppAlreadyRun == false)
                    {
                        IsAppAlreadyRun = true;                        
                        _currentApp.Run(splash as Window);                        
                    }
                }
                else
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<IList<string>>(null, EToken.NEWSHELL);
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorException(ex, "Error during RunApplication");
            }
        }

        public bool CanClose()
        {
            return true;
        }

        #endregion IAppLoader Implementation
    }
}