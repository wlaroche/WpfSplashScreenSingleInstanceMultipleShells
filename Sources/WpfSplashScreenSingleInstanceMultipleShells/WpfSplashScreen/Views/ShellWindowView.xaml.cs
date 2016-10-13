using System.ComponentModel;
using System.Windows;
using WpfSplashScreen.ViewModels;

namespace WpfSplashScreen.Views
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

    public partial class ShellWindowView : Window, IShellWindowView
    {
        public ShellWindowView(SimpleInjector.Container container, IShellWindowViewModel viewModel)
        {
            InitializeComponent();

            AppContainer = container;
            ViewModel = viewModel;
            viewModel.View = this;

            this.DataContext = ViewModel;
        }

        public SimpleInjector.Container AppContainer { get; set; }

        public IShellWindowViewModel ViewModel { get; set; }

        public bool CanClose()
        {
            bool canExit = ViewModel.CanExit();
            return canExit;
        }

        public void CloseWindow()
        {
            if (!CanClose())
                return;

            this.Close();
        }

        public void Initialize()
        {
            ViewModel.Initialize();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CanClose())
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        public void GenerateNewShellWindow()
        {
            var newShellWindow = AppContainer.GetInstance<IShellWindowView>();
            (newShellWindow as Window).Show();
        }
    }
}