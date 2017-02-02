using System;
using System.IO;
using CakeTron.Core;
using CakeTron.Core.Diagnostics;
using CakeTron.Parts;
using CakeTron.Parts.Karma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CakeTron
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            // Create the log.
            var log = new SerilogLogAdapter(new LoggerConfiguration()
                            .WriteTo.LiterateConsole()
                            .MinimumLevel.Verbose()
                            .Enrich.FromLogContext()
                            .CreateLogger());

            // Start the engine.
            var engine = Engine.Build(configuration, log, Configure);
            var handle = engine.Start();

            // Setup cancellation.
            Console.CancelKeyPress += (s, e) =>
            {
                engine.Stop();
                e.Cancel = true;
            };

            // Wait for termination.
            handle.Stopped.WaitOne();
        }

        private static void Configure(IServiceCollection services)
        {
            // Parts
            services.AddSingleton<RobotPart, PingPart>();
            services.AddSingleton<RobotPart, DirectivesPart>();
            services.AddSingleton<RobotPart, UptimePart>();
            services.AddSingleton<RobotPart, PodBayDoorsPart>();
            services.AddSingleton<RobotPart, KarmaPart>();

            // Services
            services.AddSingleton<IKarmaProvider, InMemoryKarmaProvider>();
        }
    }
}
