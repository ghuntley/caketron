using System.IO;
using CakeTron.Core;
using CakeTron.Core.Diagnostics;

namespace CakeTron.Brains.FileSystem
{
    internal sealed class FileSystemBrain : IBrainProvider
    {
        private readonly FileSystemBrainConfiguration _configuration;
        private readonly ILog _log;
        private readonly object _lock;

        public FileSystemBrain(FileSystemBrainConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
            _lock = new object();
        }

        public void Connect()
        {
            if (!_configuration.Root.Exists)
            {
                _log.Information("Creating brain root...");
                _configuration.Root.Create();
                _configuration.Root.Refresh();
                _log.Information("Brain root created.");
            }
        }

        public void Disconnect()
        {
        }

        public string Get(string key)
        {
            lock (_lock)
            {
                var file = Path.Combine(_configuration.Root.FullName, string.Concat(key, ".json"));
                if (File.Exists(file))
                {
                    return File.ReadAllText(file);
                }
                return string.Empty;
            }
        }

        public void Set(string key, string data)
        {
            lock (_lock)
            {
                var file = Path.Combine(_configuration.Root.FullName, string.Concat(key, ".json"));
                File.WriteAllText(file, data);
            }
        }
    }
}
