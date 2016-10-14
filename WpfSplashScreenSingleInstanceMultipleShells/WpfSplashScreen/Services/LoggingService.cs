using log4net;
using System;
using System.Text;
using WpfSplashScreen.Infrastructure.EventArguments;
using WpfSplashScreen.Infrastructure.Interfaces.Services;
using WpfSplashScreen.Models.Enums;

namespace WpfSplashScreen.Services
{
    public sealed class LoggingService : ILoggingService
    {
        #region Singleton

        private static volatile LoggingService instance;
        private static object syncRoot = new Object();

        public static LoggingService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LoggingService();
                    }
                }

                return instance;
            }
        }

        #endregion Singleton

        private static readonly ILog _log = LogManager.GetLogger(typeof(LoggingService));

        public LoggingService()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        #region ILoggingService Implementation

        public event EventHandler<LoggingEventArgs> LogChanged;

        public void Log(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
                return;

            _log.InfoFormat(message, args);

            if (LogChanged != null)
            {
                LogChanged(this, new LoggingEventArgs(string.Format(message, args), ELoggingCategory.Info));
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            if (!_log.IsDebugEnabled)
                return;

            _log.DebugFormat(message, args);

            if (LogChanged != null)
            {
                LogChanged(this, new LoggingEventArgs(string.Format(message, args), ELoggingCategory.Debug));
            }
        }

        public void LogInfo(string message, params object[] args)
        {
            if (!_log.IsInfoEnabled)
                return;

            _log.InfoFormat(message, args);

            if (LogChanged != null)
            {
                LogChanged(this, new LoggingEventArgs(string.Format(message, args), ELoggingCategory.Info));
            }
        }

        public void LogWarn(string message, params object[] args)
        {
            if (!_log.IsWarnEnabled)
                return;

            _log.WarnFormat(message, args);

            if (LogChanged != null)
            {
                LogChanged(this, new LoggingEventArgs(string.Format(message, args), ELoggingCategory.Warn));
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
                return;

            _log.ErrorFormat(message, args);

            if (LogChanged != null)
            {
                LogChanged(this, new LoggingEventArgs(string.Format(message, args), ELoggingCategory.Error));
            }
        }

        public void LogErrorException(Exception exception, string message, params object[] args)
        {
            if (!_log.IsErrorEnabled)
                return;

            if (exception == null)
            {
                _log.ErrorFormat(message, args);

                if (LogChanged != null)
                {
                    LogChanged(this, new LoggingEventArgs(message, ELoggingCategory.Error));
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(message, args);
                sb.AppendLine(exception.ToString());

                _log.Error(sb.ToString());

                if (LogChanged != null)
                {
                    LogChanged(this, new LoggingEventArgs(message, ELoggingCategory.Error, exception));
                }
            }
        }

        public void LogException(Exception exception)
        {
            if (!_log.IsErrorEnabled)
                return;

            _log.Error(exception.ToString());

            if (LogChanged != null)
                LogChanged(this, new LoggingEventArgs("Error.", ELoggingCategory.Error));
        }

        #endregion ILoggingService Implementation
    }
}