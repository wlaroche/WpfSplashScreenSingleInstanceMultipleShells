using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;

namespace WpfSplashScreen.Infrastructure.Interfaces.Views
{
    public interface ISplashScreenView
    {
        void Initialize();

        ISplashScreenViewModel ViewModel { get; set; }
    }
}