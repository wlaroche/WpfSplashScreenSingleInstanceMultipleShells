using System;
using System.Windows;
using System.Windows.Input;
using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;
using WpfSplashScreen.Infrastructure.Interfaces.Views;

namespace WpfSplashScreen.Views
{
    public partial class SplashScreenView : Window, ISplashScreenView
    {
        public ISplashScreenViewModel ViewModel { get; set; }

        public SplashScreenView(ISplashScreenViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        public void Initialize()
        {
            this.MouseDown += delegate { this.DragMove(); };
            this.DataContext = ViewModel;
            ViewModel.Initialize();
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.RequestCancel = true;
            base.OnClosed(e);
        }

        private void CommonCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void MaximizeWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
    }
}