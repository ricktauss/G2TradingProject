namespace ConsulServiceDiscovery.Services
{
    public static class UrlCacheServiceExtension
    {
        public static IServiceCollection AddUrlCacheService(this IServiceCollection services)
        {
            services.AddSingleton<UrlCacheService>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                return new UrlCacheService(httpClientFactory, configuration);
            });

            services.AddSingleton<IHostedService>(serviceProvider => serviceProvider.GetRequiredService<UrlCacheService>());
            
            return services;
        }
    }
}
