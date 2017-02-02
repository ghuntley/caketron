using System.Threading;

namespace CakeTron.Core.Threading
{
    public interface IManagedThreadWaitHandle
    {
        WaitHandle Running { get; }
        WaitHandle Stopped { get; }
    }
}