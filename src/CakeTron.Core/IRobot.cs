using System.Threading;

namespace CakeTron.Core
{
    public interface IRobot
    {
        WaitHandle Stopped { get; }

        void Start();
        void Stop();

        void Join();
    }
}
