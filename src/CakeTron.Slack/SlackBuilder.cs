using System;
using CakeTron.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Slack
{
    public static class SlackBuilder
    {
        public static RobotBuilder UseSlack(this RobotBuilder builder, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            // Configuration
            builder.Services.AddSingleton(new SlackConfiguration(token));

            // Adapter
            builder.Services.AddSingleton<SlackAdapter>();
            builder.Services.AddSingleton<IAdapter>(x => x.GetService<SlackAdapter>());
            builder.Services.AddSingleton<IWorker>(x => x.GetService<SlackAdapter>());

            // Broker
            builder.Services.AddSingleton<SlackBroker>();
            builder.Services.AddSingleton<IBroker>(x => x.GetService<SlackBroker>());

            return builder;
        }
    }
}
