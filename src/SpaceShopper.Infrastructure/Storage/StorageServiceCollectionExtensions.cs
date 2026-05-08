using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Storage
{
    public static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

            services.AddScoped<IFileStorageService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
                var provider = (options.Provider ?? string.Empty).Trim();

                return provider.ToLowerInvariant() switch
                {
                    "local" => new LocalFileStorageService(
                        sp.GetRequiredService<IHostEnvironment>(),
                        sp.GetRequiredService<IOptions<StorageOptions>>(),
                        sp.GetRequiredService<ILogger<LocalFileStorageService>>()),
                    "minio" => new MinIOFileStorageService(
                        sp.GetRequiredService<IOptions<StorageOptions>>(),
                        sp.GetRequiredService<ILogger<MinIOFileStorageService>>()),
                    "azureblob" => new AzureBlobFileStorageService(
                        sp.GetRequiredService<IOptions<StorageOptions>>(),
                        sp.GetRequiredService<ILogger<AzureBlobFileStorageService>>()),
                    _ => throw new InvalidOperationException(
                        $"Unknown Storage:Provider \"{options.Provider}\". Use Local, MinIO, or AzureBlob.")
                };
            });

            return services;
        }
    }
}
