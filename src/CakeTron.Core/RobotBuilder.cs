using System;
using System.Linq;
using CakeTron.Core.Diagnostics;
using CakeTron.Core.Internal;
using CakeTron.Core.Internal.Parts;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Core
{
    public sealed class RobotBuilder
    {
        public IServiceCollection Services { get; }

        public RobotBuilder()
        {
            Services = new ServiceCollection();
            ConfigureDefaultRegistrations(Services);
        }

        public IRobot Build()
        {
            if (Services.All(s => s.ServiceType != typeof(IAdapter)))
            {
                throw new InvalidOperationException("No adapter have been registered.");
            }
            if (Services.All(s => s.ServiceType != typeof(IBroker)))
            {
                throw new InvalidOperationException("No broker have been registered.");
            }

            var provider = Services.BuildServiceProvider();
            return provider.GetRequiredService<IRobot>();
        }

        private static void ConfigureDefaultRegistrations(IServiceCollection services)
        {
            // Parts
            services.AddSingleton<RobotPart, HelpCommand>();

            // Message queue
            var inbox = new MessageQueue();
            services.AddSingleton<IMessageQueue>(inbox);
            services.AddSingleton(inbox);

            // Logging
            services.AddSingleton<ILog, DefaultLog>();

            // Robot
            services.AddSingleton<IRobot, Robot>();
            services.AddSingleton<WebJobShutdownListener>();
            services.AddSingleton<MessageRouter>();
            services.AddSingleton<EventDispatcher>();
            services.AddSingleton<IEventDispatcher>(provider => provider.GetService<EventDispatcher>());
        }
    }
}
