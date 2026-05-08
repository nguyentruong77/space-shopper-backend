using SpaceShopper.Application.Dtos.Common;

namespace SpaceShopper.Application.Interfaces.Iservices.Common
{
    public interface IFileService
    {
        Task<FileUploadResponse> UploadAsync(
            Stream content,
            string originalFileName,
            string? contentType,
            long contentLength,
            Guid userId,
            CancellationToken cancellationToken);
    }
}
