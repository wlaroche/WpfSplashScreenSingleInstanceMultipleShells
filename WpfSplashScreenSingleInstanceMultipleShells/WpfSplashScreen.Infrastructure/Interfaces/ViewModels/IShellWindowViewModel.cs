using System.Collections.Generic;
using WpfSplashScreen.Infrastructure.Interfaces.Views;

namespace WpfSplashScreen.Infrastructure.Interfaces.ViewModels
{
    public interface IShellWindowViewModel
    {
        IShellWindowView View { get; set; }

        void Initialize();

        void ProcessArgs(IList<string> args);

        bool CanExit();
    }
}