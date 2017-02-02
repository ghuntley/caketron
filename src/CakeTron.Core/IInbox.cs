using System.Threading;

namespace CakeTron.Core
{
    public interface IInbox
    {
        IEvent Dequeue(CancellationToken token);
        void Enqueue(IEvent @event);
    }
}