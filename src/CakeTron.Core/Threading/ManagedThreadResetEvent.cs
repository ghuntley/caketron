using System.Threading;

namespace CakeTron.Core.Threading
{
    internal sealed class ManagedThreadResetEvent : IManagedThreadWaitHandle
    {
        public ManualResetEventSlim RunningEvent { get; }
        public ManualResetEventSlim StoppedEvent { get; }

        WaitHandle IManagedThreadWaitHandle.Running => RunningEvent.WaitHandle;
        WaitHandle IManagedThreadWaitHandle.Stopped => StoppedEvent.WaitHandle;

        public ManagedThreadResetEvent()
        {
            RunningEvent = new ManualResetEventSlim(false);
            StoppedEvent = new ManualResetEventSlim(true);
        }
    }
}
