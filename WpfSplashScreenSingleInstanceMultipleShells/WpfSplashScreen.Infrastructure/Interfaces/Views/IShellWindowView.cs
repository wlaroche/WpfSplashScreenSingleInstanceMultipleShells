using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;

namespace WpfSplashScreen.Infrastructure.Interfaces.Views
{
    public interface IShellWindowView
    {
        SimpleInjector.Container AppContainer { get; set; }

        IShellWindowViewModel ViewModel { get; set; }

        void Initialize();

        void CloseWindow();

        bool CanClose();

        void GenerateNewShellWindow();
    }
}