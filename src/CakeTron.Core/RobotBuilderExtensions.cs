using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Core
{
    public static class RobotBuilderExtensions
    {
        public static RobotBuilder AddPart<TPart>(this RobotBuilder builder)
            where TPart : RobotPart
        {
            builder.Services.AddSingleton<RobotPart, TPart>();
            return builder;
        }
    }
}
