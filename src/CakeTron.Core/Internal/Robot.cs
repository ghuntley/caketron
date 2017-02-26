using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CakeTron.Core.Internal
{
    internal sealed class Robot : IRobot
    {
        private readonly WebJobShutdownListener _listener;
        private readonly MessageRouter _router;
        private readonly List<IAdapter> _adapters;
        private readonly ManualResetEvent _stopped;
        private readonly List<Task> _tasks;

        private CancellationTokenSource _source;

        public WaitHandle Stopped => _stopped;

        public Robot(WebJobShutdownListener listener, MessageRouter router, IEnumerable<IAdapter> adapters)
        {
            _listener = listener;
            _router = router;
            _adapters = new List<IAdapter>(adapters ?? Enumerable.Empty<IAdapter>());
            _tasks = new List<Task>();

            _stopped = new ManualResetEvent(true);
        }

        public void Start()
        {
            if (_stopped.WaitOne(0))
            {
                // We're not stopped.
                _stopped.Reset();

                // Create the cancellation token source.
                _source = new CancellationTokenSource();
                _tasks.Clear();


                // Start all tasks.
                _tasks.Add(_listener.Start(_source));
                _tasks.Add(_router.Start(_source));

                // Start the adapter task.
                foreach (var adapter in _adapters)
                {
                    _tasks.Add(adapter.Start(_source));
                }

                // Configure the tasks.
                Task.WhenAll(_tasks).ContinueWith(task => _stopped.Set());
            }
        }

        public void Stop()
        {
            if (!_source.IsCancellationRequested)
            {
                _source.Cancel();
            }
        }

        public void Join()
        {
            if (!_stopped.WaitOne(0))
            {
                Task.WaitAll(_tasks.ToArray());
            }
        }
    }
}
