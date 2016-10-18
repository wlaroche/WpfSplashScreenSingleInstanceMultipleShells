using System;
using System.Threading.Tasks;
using WpfSplashScreen.Helpers;
using WpfSplashScreen.Infrastructure.Interfaces;
using WpfSplashScreen.Infrastructure.Interfaces.ViewModels;

namespace WpfSplashScreen.ViewModels
{
    public class SplashScreenViewModel : WpfSplashScreen.ViewModels.ViewModelBase, ISplashScreenViewModel
    {
        private Random _rd = new Random();

        #region Properties

        private string _appCopyright;

        public string AppCopyright
        {
            get { return _appCopyright; }
            set
            {
                _appCopyright = value;
                RaisePropertyChanged(() => AppCopyright);
            }
        }

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }
            set
            {
                _appVersion = value;
                RaisePropertyChanged(() => AppVersion);
            }
        }

        private int _progressValue;

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        #endregion Properties

        #region Contstructor

        public SplashScreenViewModel(IAppLoader appLoader)
        {
            AppVersion = appLoader.FileVersionInfo.ProductVersion;
            AppCopyright = appLoader.FileVersionInfo.LegalCopyright;
        }

        #endregion Contstructor

        private void LoadData()
        {
            try
            {
                while (ProgressValue <= 100 && RequestCancel == false)
                {
                    int newValue = _rd.Next(0, 15);
                    Utils.InvokeOnUiThread(new Action(() =>
                    {
                        ProgressValue += newValue;
                    }));
                    System.Threading.Thread.Sleep(300);

                    //Test to throw an Exception
                    //throw new Exception("Fake Error !");
                }

                Utils.InvokeOnUiThread(new Action(() =>
                {
                    if (OnLoadCompeted != null)
                        OnLoadCompeted(this, EventArgs.Empty);
                }));
            }
            catch (Exception ex)
            {
                Utils.InvokeOnUiThread(new Action(() =>
                {
                    if (OnLoadFailed != null)
                        OnLoadFailed(this, EventArgs.Empty);

                    MessengerInstance.Send<string>(ex.Message);
                }));
            }
        }

        #region ISplashScreenViewModel Implementation

        public event EventHandler OnLoadCompeted;

        public event EventHandler OnLoadFailed;

        public void Initialize()
        {
            ProgressValue = 0;

            Task.Run(new Action(() =>
            {
                LoadData();
            }));
        }

        public bool RequestCancel { get; set; }

        #endregion ISplashScreenViewModel Implementation
    }
}