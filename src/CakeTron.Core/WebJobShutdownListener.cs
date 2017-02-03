using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Threading;

namespace CakeTron.Core
{
    public sealed class WebJobShutdownListener : ManagedThread
    {
        private readonly ILog _log;
        private CancellationTokenSource _engineTokenSource;

        public override string FriendlyName => "Web job shutdown listener";

        public WebJobShutdownListener(ILog log) 
            : base(log)
        {
            _log = log;
        }

        public void SetCancellationTokenSource(CancellationTokenSource source)
        {
            _engineTokenSource = source;
        }

        protected override void Execute(object context, CancellationTokenSource source)
        {
            var type = Environment.GetEnvironmentVariable("WEBJOBS_TYPE");
            if (string.IsNullOrWhiteSpace(type))
            {
                _log.Information("Not running on Azure.");
                return;
            }

            // Get the file path to listen for.
            var path = Environment.GetEnvironmentVariable("WEBJOBS_SHUTDOWN_FILE");
            if (string.IsNullOrWhiteSpace(path))
            {
                _log.Error("No shutdown file specified.");
                return;
            }

            // File already exist?
            if (File.Exists(path))
            {
                _log.Information("Shutdown file already exist. Deleting it.");
                File.Delete(path);
            }

            _log.Information("Listning for changes in {0}...", path);

            var handles = new[] {_engineTokenSource.Token.WaitHandle, source.Token.WaitHandle};
            while (WaitHandle.WaitAny(handles, 1000) == WaitHandle.WaitTimeout)
            {
                if (File.Exists(path))
                {
                    // Abort the engine. 
                    _log.Information("Web job have been cancelled.");
                    _engineTokenSource.Cancel();
                    break;
                }
            }

            _log.Verbose("Not listening anymore.");
        }
    }
}
