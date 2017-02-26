using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Utilities;

namespace CakeTron.Core.Internal
{
    internal sealed class WebJobShutdownListener : TaskWrapper
    {
        private readonly ILog _log;

        public override string FriendlyName => "Web job shutdown listener";

        public WebJobShutdownListener(ILog log) 
            : base(log)
        {
            _log = log;
        }

        protected override Task<bool> Run(CancellationToken token)
        {
            var type = Environment.GetEnvironmentVariable("WEBJOBS_TYPE");
            if (string.IsNullOrWhiteSpace(type))
            {
                _log.Information("Not running on Azure.");
                return Task.FromResult(true);
            }

            // Get the file path to listen for.
            var path = Environment.GetEnvironmentVariable("WEBJOBS_SHUTDOWN_FILE");
            if (string.IsNullOrWhiteSpace(path))
            {
                _log.Error("No shutdown file specified.");
                return Task.FromResult(true);
            }

            // File already exist?
            if (File.Exists(path))
            {
                _log.Information("Shutdown file already exist. Deleting it.");
                File.Delete(path);
            }

            _log.Information("Listning for changes in {0}...", path);
            while (token.WaitHandle.WaitOne(1000))
            {
                if (File.Exists(path))
                {
                    // Abort the engine. 
                    _log.Information("Web job have been cancelled.");
                    return Task.FromResult(false);
                }
            }

            _log.Verbose("Not listening anymore.");
            return Task.FromResult(true);
        }
    }
}
