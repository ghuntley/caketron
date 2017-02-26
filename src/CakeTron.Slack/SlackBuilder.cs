using CakeTron.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Slack
{
    public static class SlackBuilder
    {
        public static RobotBuilder UseSlack(this RobotBuilder builder, string token)
        {
            builder.Services.AddSingleton<SlackClient>();
            builder.Services.AddSingleton<IBroker>(x => x.GetService<SlackClient>());
            builder.Services.AddSingleton<IAdapter, SlackAdapter>();
            builder.Services.AddSingleton(new SlackConfiguration(token));

            return builder;
        }
    }
}
