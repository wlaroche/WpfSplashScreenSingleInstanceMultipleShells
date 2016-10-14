using System;
using WpfSplashScreen.Models.Enums;

namespace WpfSplashScreen.Infrastructure.EventArguments
{
    public class LoggingEventArgs : EventArgs
    {
        public string Message { get; set; }

        public ELoggingCategory Category { get; set; }

        public Exception LogException { get; set; }

        public LoggingEventArgs(string message)
        {
            Message = message;
        }

        public LoggingEventArgs(string message, ELoggingCategory category)
            : this(message)
        {
            Category = category;
        }

        public LoggingEventArgs(string message, ELoggingCategory category, Exception logException)
            : this(message, category)
        {
            LogException = logException;
        }
    }
}