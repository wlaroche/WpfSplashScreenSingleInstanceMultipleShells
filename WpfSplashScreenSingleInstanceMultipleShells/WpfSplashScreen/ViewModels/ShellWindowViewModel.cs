using GalaSoft.MvvmLight.CommandWpf;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using WpfSplashScreen.Helpers;
using WpfSplashScreen.Infrastructure.Interfaces.Services;
using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;
using WpfSplashScreen.Infrastructure.Interfaces.Views;
using WpfSplashScreen.Models.Enums;

namespace WpfSplashScreen.ViewModels
{
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
                (AbortComputeCommand as RelayCommand).RaiseCanExecuteChanged();

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

        private int _computeProgressValue;

        public int ComputeProgressValue
        {
            get { return _computeProgressValue; }
            set
            {
                _computeProgressValue = value;
                RaisePropertyChanged(() => ComputeProgressValue);
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

        private ICommand _abortComputeCommand;

        public ICommand AbortComputeCommand
        {
            get
            {
                if (_abortComputeCommand == null)
                {
                    _abortComputeCommand = new RelayCommand(AbortCompute, CanExecuteAbortCompute);
                }
                return _abortComputeCommand;
            }
        }

        #endregion Commands

        public ShellWindowViewModel(Container container, IComputeService computeService)
        {
            Container = container;
            ComputeService = computeService;

            ComputeService.OnCompputeCompleted += (s, e) =>
            {
                Utils.InvokeOnUiThread(() =>
                {
                    IsGeneralComputing = false;
                });
            };

            ComputeService.OnComputeAborted += (s, e) =>
            {
                Utils.InvokeOnUiThread(() =>
                {
                    IsGeneralComputing = false;
                });
            };

            ComputeService.OnCompputeProgressChanged += (s, e) =>
            {
                Utils.InvokeOnUiThread(() =>
                {
                    ComputeProgressValue = e.ProgressValue;
                });
            };

            IsGeneralComputing = computeService.IsComputing;
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
            ComputeService.Compute(_instanceId);
        }

        private void AbortCompute()
        {
            if (CanExecuteAbortCompute() == true)
                ComputeService.AbortCompute();
        }

        private void ExitWindowAction()
        {
            if (CanExit() == true)
                View.CloseWindow();
        }

        private bool CanExecuteCompute()
        {
            return (ComputeService.IsComputing != true);
        }

        private bool CanExecuteAbortCompute()
        {
            return (ComputeService.IsComputing == true && IsLocalComputing == true);
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