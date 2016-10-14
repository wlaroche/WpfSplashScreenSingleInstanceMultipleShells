using System;
using WpfSplashScreen.Infrastructure.EventArguments;

namespace WpfSplashScreen.Infrastructure.Interfaces.Services
{
    public interface ILoggingService
    {
        event EventHandler<LoggingEventArgs> LogChanged;

        void Log(string message, params object[] args);

        void LogDebug(string message, params object[] args);

        void LogInfo(string message, params object[] args);

        void LogWarn(string message, params object[] args);

        void LogError(string message, params object[] args);

        void LogErrorException(Exception exception, string message, params object[] args);

        void LogException(Exception exception);
    }
}