using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Storage
{
    public sealed class AzureBlobFileStorageService(
        IOptions<StorageOptions> storageOptions,
        ILogger<AzureBlobFileStorageService> logger) : IFileStorageService
    {
        private readonly IOptions<StorageOptions> _storageOptions = storageOptions;
        private readonly ILogger<AzureBlobFileStorageService> _logger = logger;

        public async Task<FileUploadResponse> SaveAsync(
            Stream content,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken)
        {
            var opts = _storageOptions.Value;
            var az = opts.AzureBlob;

            if (string.IsNullOrWhiteSpace(az.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Azure Blob storage is selected but ConnectionString is empty. Configure Storage:AzureBlob:ConnectionString when ready.");
            }

            var containerName = string.IsNullOrWhiteSpace(az.ContainerName) ? opts.ContainerOrBucket : az.ContainerName;

            var service = new BlobServiceClient(az.ConnectionString);
            var container = service.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var key = objectKey.Replace('\\', '/');
            var blob = container.GetBlobClient(key);

            try
            {
                var headers = new BlobHttpHeaders { ContentType = contentType };
                await blob.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken)
                    .ConfigureAwait(false);
                var props = await blob.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                var baseUrl = !string.IsNullOrWhiteSpace(az.PublicBaseUrl) ? az.PublicBaseUrl : opts.PublicBaseUrl;
                var url = StoragePublicUrl.Combine(baseUrl, key);
                return new FileUploadResponse
                {
                    Url = url,
                    Filename = Path.GetFileName(key),
                    Size = props.Value.ContentLength,
                    ObjectKey = objectKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure Blob upload failed for container {Container}, key {Key}", containerName, key);
                throw;
            }
        }
    }
}
