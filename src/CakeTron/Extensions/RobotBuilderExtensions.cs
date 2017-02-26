using CakeTron.Core;
using CakeTron.Core.Diagnostics;
using CakeTron.Diagnostics;
using CakeTron.Parts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    public static class RobotBuilderExtensions
    {
        public static RobotBuilder UseConfiguration(this RobotBuilder builder, IConfigurationRoot configuration)
        {
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddSingleton(configuration);

            return builder;
        }

        public static RobotBuilder UseKarma(this RobotBuilder builder)
        {
            builder.AddPart<KarmaPart>();

            return builder;
        }

        public static RobotBuilder UseDefaultRobotParts(this RobotBuilder builder)
        {
            return builder
                .AddPart<PingPart>()
                .AddPart<DirectivesPart>()
                .AddPart<UptimePart>()
                .AddPart<PodBayDoorsPart>();
        }

        public static RobotBuilder UseSerilogConsole(this RobotBuilder builder, LogEventLevel level)
        {
            builder.Services.AddSingleton<ILog>(
                new SerilogLogAdapter(
                    new LoggerConfiguration()
                        .WriteTo.LiterateConsole()
                        .MinimumLevel.Is(level)
                        .Enrich.FromLogContext()
                        .CreateLogger()));

            return builder;
        }
    }
}
