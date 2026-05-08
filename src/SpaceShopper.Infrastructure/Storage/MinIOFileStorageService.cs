using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Storage
{
    public sealed class MinIOFileStorageService : IFileStorageService, IDisposable
    {
        private readonly IOptions<StorageOptions> _storageOptions;
        private readonly ILogger<MinIOFileStorageService> _logger;
        private readonly AmazonS3Client _client;

        public MinIOFileStorageService(
            IOptions<StorageOptions> storageOptions,
            ILogger<MinIOFileStorageService> logger)
        {
            _storageOptions = storageOptions;
            _logger = logger;
            var opts = storageOptions.Value;
            var m = opts.MinIO;

            if (string.IsNullOrWhiteSpace(m.AccessKey) || string.IsNullOrWhiteSpace(m.SecretKey))
            {
                throw new InvalidOperationException(
                    "MinIO storage is selected but AccessKey or SecretKey is missing. Set Storage:MinIO:AccessKey and Storage:MinIO:SecretKey (user secrets or environment variables).");
            }

            var cfg = new AmazonS3Config
            {
                ServiceURL = m.ServiceUrl,
                ForcePathStyle = m.ForcePathStyle
            };

            _client = new AmazonS3Client(m.AccessKey, m.SecretKey, cfg);
        }

        public async Task<FileUploadResponse> SaveAsync(
            Stream content,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken)
        {
            var opts = _storageOptions.Value;
            var m = opts.MinIO;
            var bucket = string.IsNullOrWhiteSpace(m.BucketName) ? opts.ContainerOrBucket : m.BucketName;

            long size;
            if (content.CanSeek)
            {
                size = content.Length;
            }
            else
            {
                size = 0;
            }

            var baseUrl = !string.IsNullOrWhiteSpace(m.PublicBaseUrl) ? m.PublicBaseUrl : opts.PublicBaseUrl;
            var url = StoragePublicUrl.Combine(baseUrl, objectKey);

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = objectKey.Replace('\\', '/'),
                    InputStream = content,
                    ContentType = contentType,
                    AutoCloseStream = false,
                    UseChunkEncoding = false
                };

                if (size > 0)
                {
                    request.Headers.ContentLength = size;
                }

                await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);
                var filename = Path.GetFileName(objectKey);
                var resolvedSize = size > 0
                    ? size
                    : content.CanSeek
                        ? content.Length
                        : 0;

                return new FileUploadResponse
                {
                    Url = url,
                    Filename = filename,
                    Size = resolvedSize,
                    ObjectKey = objectKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MinIO PutObject failed for bucket {Bucket}, key {Key}", bucket, objectKey);
                throw;
            }
        }

        public void Dispose() => _client.Dispose();
    }
}
