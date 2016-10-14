using System;

namespace WpfSplashScreen.Infrastructure.EventArguments
{
    public class ComputeProgressEventArgs : EventArgs
    {
        public int ProgressValue { get; set; }

        public ComputeProgressEventArgs(int progressValue)
        {
            ProgressValue = progressValue;
        }
    }
}