using SimpleInjector;
using System.Collections.Generic;
using System.Diagnostics;
using WpfSplashScreen.Infrastructure.Interfaces.Services;

namespace WpfSplashScreen.Infrastructure.Interfaces
{
    public interface IAppLoader
    {
        ILoggingService Logger { get; }

        FileVersionInfo FileVersionInfo { get; }

        Container Container { get; }

        string ExecutableDirPath { get; }

        object DependencyResolver { get; }

        void Initialize();

        void RunApplication(bool showSplashScreen, IList<string> args);

        bool CanClose();
    }
}