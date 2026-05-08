using SpaceShopper.Application.Dtos.Common;

namespace SpaceShopper.Application.Interfaces.Iservices.Common
{
    /// <summary>
    /// Ghi nội dung file lên storage (đĩa cục bộ hoặc cloud). Không validate nghiệp vụ upload.
    /// </summary>
    public interface IFileStorageService
    {
        /// <param name="objectKey">Đường dẫn tương đối an toàn trong bucket/container (ví dụ uploads/2026/05/{userId}/file.webp).</param>
        Task<FileUploadResponse> SaveAsync(
            Stream content,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken);
    }
}
