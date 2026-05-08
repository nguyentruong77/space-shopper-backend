using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Common.Files;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Application.Services.Common
{
    public sealed class FileService(
        IOptions<StorageOptions> storageOptions,
        IFileStorageService fileStorage,
        ILogger<FileService> logger) : IFileService
    {
        private readonly StorageOptions _options = storageOptions.Value;
        private readonly IFileStorageService _fileStorage = fileStorage;
        private readonly ILogger<FileService> _logger = logger;

        public async Task<FileUploadResponse> UploadAsync(
            Stream content,
            string originalFileName,
            string? contentType,
            long contentLength,
            Guid userId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(originalFileName) || contentLength <= 0)
            {
                _logger.LogWarning("File upload rejected: missing or empty file for user {UserId}", userId);
                throw new ValidationException(ErrorCodes.File.MissingOrEmpty, ErrorMessages.File.MissingOrEmpty);
            }

            if (contentLength > _options.MaxFileSizeBytes)
            {
                _logger.LogWarning(
                    "File upload rejected: size {Size} exceeds max {Max} for user {UserId}",
                    contentLength,
                    _options.MaxFileSizeBytes,
                    userId);
                throw new ValidationException(ErrorCodes.File.SizeExceeded, ErrorMessages.File.SizeExceeded);
            }

            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_options.AllowedExtensions.Any(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("File upload rejected: extension {Ext} not allowed for user {UserId}", ext, userId);
                throw new ValidationException(ErrorCodes.File.ExtensionNotAllowed, ErrorMessages.File.ExtensionNotAllowed);
            }

            const int peekSize = 32;
            var peekBuffer = new byte[peekSize];
            var read = await content.ReadAsync(peekBuffer.AsMemory(0, peekSize), cancellationToken);
            if (read < 12 || !FileContentSignatureValidator.MatchesExtension(peekBuffer.AsSpan(0, read), ext))
            {
                _logger.LogWarning("File upload rejected: content signature mismatch for extension {Ext}, user {UserId}", ext, userId);
                throw new ValidationException(ErrorCodes.File.ContentSignatureMismatch, ErrorMessages.File.ContentSignatureMismatch);
            }

            var safeName = $"{Guid.NewGuid():N}{ext}";
            var now = DateTime.UtcNow;
            var objectKey = FormattableString.Invariant($"uploads/{now:yyyy}/{now:MM}/{userId:N}/{safeName}");

            var resolvedContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;

            FileUploadResponse result;
            if (content.CanSeek)
            {
                content.Position = 0;
                result = await _fileStorage.SaveAsync(content, objectKey, resolvedContentType, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var capacity = (int)Math.Min(contentLength, int.MaxValue);
                await using var ms = new MemoryStream(capacity);
                await ms.WriteAsync(peekBuffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                await content.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
                ms.Position = 0;
                result = await _fileStorage.SaveAsync(ms, objectKey, resolvedContentType, cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.LogInformation(
                "File uploaded: user {UserId}, filename {Filename}, size {Size}, contentType {ContentType}, url {Url}",
                userId,
                result.Filename,
                result.Size,
                resolvedContentType,
                result.Url);

            return result;
        }
    }
}
