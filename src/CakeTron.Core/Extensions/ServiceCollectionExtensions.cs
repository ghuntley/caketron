using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    internal static class ServiceCollectionExtensions
    {
        public static void ConfigurePoco<TConfig>(this IServiceCollection services, IConfiguration configuration) 
            where TConfig : class, new()
        {
            var config = new TConfig();
            configuration.Bind(config);

            services.AddSingleton(config);
        }
    }
}
