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

            builder.Services.AddSingleton(new GitterConfiguration() { Token = token });
            builder.Services.AddSingleton<GitterClient>();
            builder.Services.AddSingleton<IBroker>(x => x.GetService<GitterClient>());
            builder.Services.AddSingleton<IAdapter, GitterAdapter>();

            return builder;
        }
    }
}
