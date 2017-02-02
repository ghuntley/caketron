using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Parts;
using CakeTron.Core.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Core
{
    public sealed class Engine : ManagedThread<object>
    {
        private readonly ILog _log;
        private readonly IInbox _inbox;
        private readonly StreamClient _streamClient;
        private readonly EventDispatcher _dispatcher;

        public override string FriendlyName => "Engine";

        public Engine(EngineSettings settings, IInbox inbox, ILog log, IEnumerable<RobotPart> handlers)
            : base(log)
        {
            if (string.IsNullOrWhiteSpace(settings.Token))
            {
                throw new ArgumentException("No Gitter API key set.", nameof(settings));
            }

            _log = log;

            var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);

            var gitter = new GitterClient(client);

            _inbox = inbox;
            _streamClient = new StreamClient(settings, _inbox, client, gitter, _log);
            _dispatcher = new EventDispatcher(gitter, _log, handlers);
        }

        public static Engine Build(IConfiguration configuration, ILog log, Action<IServiceCollection> services)
        {
            var collection = new ServiceCollection();

            // Add handlers.
            collection.AddSingleton<RobotPart, HelpCommand>();

            // Logging
            collection.AddSingleton(log);

            // Inbox
            collection.AddSingleton<IInbox, Inbox>();

            // Engine
            collection.AddSingleton<Engine>();
            collection.ConfigurePoco<EngineSettings>(configuration.GetSection("CakeTron"));

            // Add custom registrations.
            services.Invoke(collection);

            // Resolve the engine.
            var provider = collection.BuildServiceProvider();
            return provider.GetRequiredService<Engine>();
        }

        protected override object CreateContext()
        {
            return null;
        }

        protected override void Execute(object context, CancellationTokenSource source)
        {
            try
            {
                // Start the stream client.
                var handle = _streamClient.Start();
                handle.Running.WaitOne();

                while (true)
                {
                    var message = _inbox.Dequeue(source.Token);
                    message?.Accept(_dispatcher);
                }
            }
            catch (OperationCanceledException)
            {
                _log.Information("Engine was requested to stop.");
            }
            finally
            {
                _streamClient.Stop();
            }
        }
    }
}
