using System;
using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;

namespace CakeTron.Core.Utilities
{
    public abstract class TaskWrapper
    {
        private readonly ILog _log;
        private CancellationTokenSource _external;
        private CancellationTokenSource _internal;

        public abstract string FriendlyName { get; }
        public Task Task { get; private set; }

        protected TaskWrapper(ILog log)
        {
            _log = log;
        }

        protected abstract Task<bool> Run(CancellationToken token);

        public Task Start(CancellationTokenSource source)
        {
            _log.Verbose("Starting {0}...", FriendlyName.ToLowerInvariant());

            _external = source;
            _internal = new CancellationTokenSource();

            Task = Task.Factory.StartNew(() =>
            {
                try
                {
                    _log.Debug("{0} started.", FriendlyName);

                    var linked = CancellationTokenSource.CreateLinkedTokenSource(_external.Token, _internal.Token);
                    if (!Run(linked.Token).GetAwaiter().GetResult())
                    {
                        _external.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    _log.Verbose("{0} aborted.", FriendlyName);
                }
                catch (Exception ex)
                {
                    _log.Error("{0}: {1} ({2})", FriendlyName, ex.Message, ex.GetType().FullName);
                    _external.Cancel();
                }
                finally
                {
                    _log.Debug("{0} stopped.", FriendlyName);
                }
            }, TaskCreationOptions.LongRunning);

            return Task;
        }
    }
}