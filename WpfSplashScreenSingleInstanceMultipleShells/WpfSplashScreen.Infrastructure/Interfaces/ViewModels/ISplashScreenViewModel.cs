using System;

namespace WpfSplashScreen.Infrastructure.Interfaces.ViewModels
{
    public interface ISplashScreenViewModel
    {
        event EventHandler OnLoadCompeted;

        event EventHandler OnLoadFailed;

        void Initialize();

        bool RequestCancel { get; set; }
    }
}