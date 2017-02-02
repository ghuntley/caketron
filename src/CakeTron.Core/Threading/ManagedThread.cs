using System.Threading;
using CakeTron.Core.Diagnostics;

namespace CakeTron.Core.Threading
{
    public abstract class ManagedThread<TContext>
        where TContext : class
    {
        private readonly ILog _log;
        private readonly ManagedThreadResetEvent _handle;
        private readonly object _lock;
        private CancellationTokenSource _source;
        private Thread _thread;

        protected ManagedThread(ILog log)
        {
            _log = log;
            _handle = new ManagedThreadResetEvent();
            _lock = new object();
        }

        protected abstract TContext CreateContext();

        public abstract string FriendlyName { get; }

        protected abstract void Execute(TContext context, CancellationTokenSource source);

        public IManagedThreadWaitHandle Start()
        {
            lock (_lock)
            {
                if (!_handle.RunningEvent.WaitHandle.WaitOne(0))
                {
                    _log.Verbose("Starting {0} thread...", FriendlyName.ToLower());
                    _source = new CancellationTokenSource();
                    _thread = new Thread(Run) { Name = FriendlyName };
                    _thread.Start(CreateContext());
                    _handle.RunningEvent.WaitHandle.WaitOne();
                    _log.Verbose("{0} thread started.", FriendlyName);
                }
            }
            return _handle;
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_handle.RunningEvent.WaitHandle.WaitOne(0))
                {
                    _source.Cancel();
                    _handle.StoppedEvent.Wait();
                    _source.Dispose();
                }
            }
        }

        public void Run(object args)
        {
            try
            {
                // Reset signaling.
                _handle.RunningEvent.Set();
                _handle.StoppedEvent.Reset();

                Execute(args as TContext, _source);
            }
            finally
            {
                _handle.StoppedEvent.Set();
                _handle.RunningEvent.Reset();
            }
        }
    }
}