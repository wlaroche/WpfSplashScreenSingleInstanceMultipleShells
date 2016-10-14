using System;
using WpfSplashScreen.Infrastructure.EventArguments;

namespace WpfSplashScreen.Infrastructure.Interfaces.Services
{
    public interface IComputeService
    {
        bool CanCompute { get; }

        bool IsComputing { get; }

        void AbortCompute();

        void Compute(string processId);

        event EventHandler OnCompputeCompleted;

        event EventHandler OnComputeAborted;

        event EventHandler<ComputeProgressEventArgs> OnCompputeProgressChanged;
    }
}