using System;
using System.Threading.Tasks;
using WpfSplashScreen.Helpers;

namespace WpfSplashScreen.ViewModels
{
    public interface ISplashScreenViewModel
    {
        event EventHandler OnLoadCompeted;

        event EventHandler OnLoadFailed;

        void Initialize();

        bool RequestCancel { get; set; }
    }

    public class SplashScreenViewModel : WpfSplashScreen.ViewModels.ViewModelBase, ISplashScreenViewModel
    {
        public event EventHandler OnLoadCompeted;

        public event EventHandler OnLoadFailed;

        private Random _rd = new Random();

        public bool RequestCancel { get; set; }

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

        public SplashScreenViewModel()
        {
        }

        public void Initialize()
        {
            ProgressValue = 0;

            Task.Run(new Action(() =>
            {
                LoadData();
            }));
        }

        private void LoadData()
        {
            try
            {
                while (ProgressValue <= 100 && RequestCancel == false)
                {
                    int newValue = _rd.Next(0, 10);
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
    }
}