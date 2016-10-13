using System;
using System.Windows;

namespace WpfSplashScreen.Helpers
{
    public static class Utils
    {
        public static void InvokeOnUiThread(Action callback)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null)
                return;

            dispatcher.Invoke(callback);
        }
    }
}