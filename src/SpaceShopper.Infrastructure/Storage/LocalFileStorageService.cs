using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Storage
{
    public sealed class LocalFileStorageService(
        IHostEnvironment hostEnvironment,
        IOptions<StorageOptions> storageOptions,
        ILogger<LocalFileStorageService> logger) : IFileStorageService
    {
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IOptions<StorageOptions> _storageOptions = storageOptions;
        private readonly ILogger<LocalFileStorageService> _logger = logger;

        public async Task<FileUploadResponse> SaveAsync(
            Stream content,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken)
        {
            var opts = _storageOptions.Value;
            var relativeRoot = opts.Local.RelativeRoot.Trim().TrimStart(Path.DirectorySeparatorChar, '/');
            var root = Path.GetFullPath(Path.Combine(_hostEnvironment.ContentRootPath, relativeRoot));
            var relativeKey = objectKey.Replace('/', Path.DirectorySeparatorChar);
            var physical = Path.GetFullPath(Path.Combine(root, relativeKey));

            if (!physical.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Rejected object key outside storage root: {ObjectKey}", objectKey);
                throw new InvalidOperationException("Invalid storage key.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(physical)!);

            try
            {
                await using var fs = new FileStream(physical, FileMode.Create, FileAccess.Write, FileShare.None, 81920,
                    FileOptions.Asynchronous | FileOptions.SequentialScan);
                await content.CopyToAsync(fs, cancellationToken).ConfigureAwait(false);
                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
                var size = fs.Length;
                var url = StoragePublicUrl.Combine(opts.PublicBaseUrl, objectKey.Replace('\\', '/'));
                var filename = Path.GetFileName(physical);
                return new FileUploadResponse
                {
                    Url = url,
                    Filename = filename,
                    Size = size,
                    ObjectKey = objectKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Local file write failed for key {ObjectKey}", objectKey);
                throw;
            }
        }
    }
}
