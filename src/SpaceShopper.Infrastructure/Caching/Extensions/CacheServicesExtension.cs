using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Application.Common.Caching;
using SpaceShopper.Infrastructure.Caching.Services;
using SpaceShopper.Infrastructure.Caching.Configuration;

namespace SpaceShopper.Infrastructure.Caching.Extensions
{
    public static class CacheServicesExtension
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheSettings = new CacheSetting();
            configuration.GetSection(CacheSetting.SectionName).Bind(cacheSettings);
            services.AddSingleton<IOptions<CacheSetting>>(_ => Options.Create(cacheSettings));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"];
                options.InstanceName = "spaceshopper:";
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
