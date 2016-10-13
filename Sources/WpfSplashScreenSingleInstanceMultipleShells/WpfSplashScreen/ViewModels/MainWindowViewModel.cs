using GalaSoft.MvvmLight.CommandWpf;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfSplashScreen.Helpers;
using WpfSplashScreen.Models.Enums;
using WpfSplashScreen.Models.Events;
using WpfSplashScreen.Services;
using WpfSplashScreen.Views;

namespace WpfSplashScreen.ViewModels
{
    public interface IShellWindowViewModel
    {
        IShellWindowView View { get; set; }

        void Initialize();

        void ProcessArgs(IList<string> args);

        bool CanExit();
    }

    public class ShellWindowViewModel : WpfSplashScreen.ViewModels.ViewModelBase, IShellWindowViewModel
    {
        private string _instanceId = Guid.NewGuid().ToString();

        public IComputeService ComputeService { get; private set; }

        public Container Container { get; set; }

        public IShellWindowView View { get; set; }

        private bool _isGeneralComputing;

        public bool IsGeneralComputing
        {
            get { return _isGeneralComputing; }
            set
            {
                if (_isGeneralComputing == value)
                    return;
                _isGeneralComputing = value;
                RaisePropertyChanged(() => IsGeneralComputing);
                (ComputeCommand as RelayCommand).RaiseCanExecuteChanged();

                IsLocalComputing = value;
            }
        }

        private bool _isLocalComputing;

        public bool IsLocalComputing
        {
            get { return _isLocalComputing; }
            set
            {
                if (_isLocalComputing == value)
                    return;
                _isLocalComputing = value;
                RaisePropertyChanged(() => IsLocalComputing);
            }
        }

        #region Commands

        private ICommand _newWindowCommand;

        public ICommand NewWindowCommand
        {
            get
            {
                if (_newWindowCommand == null)
                {
                    _newWindowCommand = new RelayCommand(() => NewWindowAction());
                }
                return _newWindowCommand;
            }
        }

        private ICommand _exitWindowCommand;

        public ICommand ExitWindowCommand
        {
            get
            {
                if (_exitWindowCommand == null)
                {
                    _exitWindowCommand = new RelayCommand(() => ExitWindowAction());
                }
                return _exitWindowCommand;
            }
        }

        private ICommand _computeCommand;

        public ICommand ComputeCommand
        {
            get
            {
                if (_computeCommand == null)
                {
                    _computeCommand = new RelayCommand(Compute, CanExecuteCompute);
                }
                return _computeCommand;
            }
        }

        #endregion Commands

        public ShellWindowViewModel(Container container, IComputeService computeService)
        {
            Container = container;
            ComputeService = computeService;

            MessengerInstance.Register<ComputeEvent>(this, EToken.COMPUTE, (evt) =>
            {
                IsGeneralComputing = evt.IsComputing;
                if (evt.InstanceId == _instanceId)
                    IsLocalComputing = evt.IsComputing;
                else
                    IsLocalComputing = false;
            });

            IsGeneralComputing = computeService.CanCompute;
            IsLocalComputing = false;
        }

        public void Initialize()
        {
        }

        public void ProcessArgs(IList<string> args)
        {
            if (args == null || args.Count == 0)
                return;

            foreach (var item in args)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("Args: " + item, EToken.MESSAGEBOX);
            }
        }

        private void NewWindowAction()
        {
            MessengerInstance.Send<IList<string>>(null, EToken.NEWSHELL);
        }

        private void Compute()
        {
            IsGeneralComputing = true;
            MessengerInstance.Send<ComputeEvent>(new ComputeEvent(IsGeneralComputing, _instanceId), EToken.COMPUTE);

            Task.Run(new Action(() =>
            {
                Thread.Sleep(6000);
                Utils.InvokeOnUiThread(() =>
                {
                    IsGeneralComputing = false;
                    MessengerInstance.Send<ComputeEvent>(new ComputeEvent(IsGeneralComputing, _instanceId), EToken.COMPUTE);
                });
            }));
        }

        private void ExitWindowAction()
        {
            if (CanExit() == true)
                View.CloseWindow();
        }

        private bool CanExecuteCompute()
        {
            return (IsGeneralComputing != true);
        }

        public bool CanExit()
        {
            if (IsLocalComputing == true)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("Please end of process before to exit !", EToken.MESSAGEBOX);
                return false;
            }
            return true;
        }
    }
}