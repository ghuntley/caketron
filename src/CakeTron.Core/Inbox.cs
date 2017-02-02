using System.Collections.Concurrent;
using System.Threading;

namespace CakeTron.Core
{
    internal sealed class Inbox : IInbox
    {
        private readonly BlockingCollection<IEvent> _queue;

        public Inbox()
        {
            _queue = new BlockingCollection<IEvent>(new ConcurrentQueue<IEvent>());
        }

        public void Enqueue(IEvent @event)
        {
            if (!_queue.IsAddingCompleted)
            {
                _queue.Add(@event);
            }
        }

        public IEvent Dequeue(CancellationToken token)
        {
            IEvent entry;
            return _queue.TryTake(out entry, Timeout.Infinite, token) ? entry : null;
        }
    }
}
