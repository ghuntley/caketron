using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Utilities;

namespace CakeTron.Core.Internal
{
    internal sealed class Robot : IRobot
    {
        private readonly List<IAdapter> _adapters;
        private readonly List<IWorker> _workers;
        private readonly ILog _log;
        private readonly ManualResetEvent _stopped;
        private readonly List<Task> _tasks;

        private CancellationTokenSource _source;

        public IReadOnlyList<IAdapter> Adapters => _adapters;
        public WaitHandle Stopped => _stopped;

        public Robot(IEnumerable<IAdapter> adapters, IEnumerable<IWorker> workers, ILog log)
        {
            _adapters = new List<IAdapter>(adapters ?? Enumerable.Empty<IAdapter>());
            _workers = new List<IWorker>(workers ?? Enumerable.Empty<IWorker>());
            _log = log;

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

                // Start all tasks.
                _tasks.Clear();
                foreach (var worker in _workers)
                {
                    _tasks.Add(new TaskWrapper(worker, _log).Start(_source));
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
