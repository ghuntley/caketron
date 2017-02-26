using System;
using CakeTron.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Gitter
{
    public static class GitterBuilder
    {
        public static RobotBuilder UseGitter(this RobotBuilder builder, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            // Configuration
            builder.Services.AddSingleton(new GitterConfiguration() { Token = token });

            // Adapter
            builder.Services.AddSingleton<GitterAdapter>();
            builder.Services.AddSingleton<IAdapter>(x => x.GetService<GitterAdapter>());
            builder.Services.AddSingleton<IWorker>(x => x.GetService<GitterAdapter>());

            // Broker
            builder.Services.AddSingleton<GitterBroker>();
            builder.Services.AddSingleton<IBroker>(x => x.GetService<GitterBroker>());

            return builder;
        }
    }
}
