using GalaSoft.MvvmLight.Messaging;
using WpfSplashScreen.Models.Enums;
using WpfSplashScreen.Models.Events;

namespace WpfSplashScreen.Services
{
    public interface IComputeService
    {
        bool CanCompute { get; set; }
    }

    public class ComputeService : IComputeService
    {
        public bool CanCompute { get; set; }

        public ComputeService()
        {
            Messenger.Default.Register<ComputeEvent>(this, EToken.COMPUTE, (evt) =>
            {
                CanCompute = evt.IsComputing;
            });
        }
    }
}