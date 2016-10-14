using System;
using System.Windows;
using System.Windows.Input;
using WpfSplashScreen.ViewModels;

namespace WpfSplashScreen.Views
{
    public interface ISplashScreenView
    {
        void Initialize();

        ISplashScreenViewModel ViewModel { get; set; }
    }

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

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //SystemCommands.CloseWindow(this);

            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}