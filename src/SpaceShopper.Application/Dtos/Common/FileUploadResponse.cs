namespace SpaceShopper.Application.Dtos.Common
{
    public sealed class FileUploadResponse
    {
        public required string Url { get; init; }

        public required string Filename { get; init; }

        public required long Size { get; init; }

        /// <summary>
        /// Object key/path tương đối trong bucket/container (để lưu vào DB).
        /// </summary>
        public required string ObjectKey { get; init; }
    }
}
