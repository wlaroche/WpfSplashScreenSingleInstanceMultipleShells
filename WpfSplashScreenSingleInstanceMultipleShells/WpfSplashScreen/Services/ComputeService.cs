using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using WpfSplashScreen.Infrastructure.EventArguments;
using WpfSplashScreen.Infrastructure.Interfaces.Services;
using WpfSplashScreen.Models.Enums;
using WpfSplashScreen.Models.Events;

namespace WpfSplashScreen.Services
{
    public class ComputeService : IComputeService
    {
        private bool _requestAbort = false;
        private Random _rd = new Random();
        private string _currentProcessId;

        public event EventHandler OnCompputeCompleted;

        public event EventHandler OnComputeAborted;

        public event EventHandler<ComputeProgressEventArgs> OnCompputeProgressChanged;

        public bool CanCompute
        {
            get
            {
                return !IsComputing;
            }
        }

        private bool _isComputing;

        public bool IsComputing
        {
            get { return _isComputing; }
            private set
            {
                if (_isComputing == value)
                    return;

                _isComputing = value;
                Messenger.Default.Send<ComputeEvent>(new ComputeEvent(value, _currentProcessId), EToken.COMPUTE);
            }
        }

        public ComputeService()
        {
            Messenger.Default.Register<ComputeEvent>(this, EToken.COMPUTE, (evt) =>
            {
                IsComputing = evt.IsComputing;
            });
        }

        public void AbortCompute()
        {
            _requestAbort = true;
        }

        public void Compute(string processId)
        {
            _currentProcessId = processId;
            IsComputing = true;

            if (OnCompputeProgressChanged != null)
                OnCompputeProgressChanged(this, new ComputeProgressEventArgs(0));

            Task.Run(new Action(() =>
            {
                int progressValue = 0;

                while (progressValue <= 100 && _requestAbort == false)
                {
                    int newValue = _rd.Next(0, 15);
                    progressValue += newValue;
                    System.Threading.Thread.Sleep(250);
                    if (OnCompputeProgressChanged != null)
                        OnCompputeProgressChanged(this, new ComputeProgressEventArgs(progressValue));
                }

                IsComputing = false;
                _currentProcessId = null;

                if (_requestAbort == true)
                {
                    _requestAbort = false;
                    if (OnComputeAborted != null)
                        OnComputeAborted(this, EventArgs.Empty);
                }
                else
                {
                    if (OnCompputeCompleted != null)
                        OnCompputeCompleted(this, EventArgs.Empty);
                }
            }));
        }
    }
}