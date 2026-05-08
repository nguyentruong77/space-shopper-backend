namespace SpaceShopper.Application.Common.Settings
{
    /// <summary>
    /// Cấu hình lưu trữ file, bind từ section "Storage".
    /// </summary>
    public sealed class StorageOptions
    {
        public const string SectionName = "Storage";

        /// <summary>Giới hạn HTTP request cho action upload (lớn hơn <see cref="MaxFileSizeBytes"/> để chừa multipart overhead).</summary>
        public const long MaxUploadRequestBytes = 10_485_760;

        /// <summary>Local | MinIO | AzureBlob</summary>
        public string Provider { get; set; } = "Local";

        public long MaxFileSizeBytes { get; set; } = 5_242_880;

        public List<string> AllowedExtensions { get; set; } = new() { ".jpg", ".jpeg", ".png", ".webp" };

        /// <summary>URL gốc (không có slash cuối) ghép với object key cho client.</summary>
        public string PublicBaseUrl { get; set; } = string.Empty;

        /// <summary>Tên bucket/container mặc định khi section con để trống.</summary>
        public string ContainerOrBucket { get; set; } = "spaceshopper-files";

        public MinIOStorageOptions MinIO { get; set; } = new();

        public AzureBlobStorageOptions AzureBlob { get; set; } = new();

        public LocalStorageOptions Local { get; set; } = new();
    }

    public sealed class MinIOStorageOptions
    {
        public string ServiceUrl { get; set; } = "http://localhost:9000";

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public bool ForcePathStyle { get; set; } = true;

        /// <summary>Nếu set, ghi đè <see cref="StorageOptions.PublicBaseUrl"/> khi build URL MinIO.</summary>
        public string PublicBaseUrl { get; set; } = string.Empty;
    }

    public sealed class AzureBlobStorageOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string ContainerName { get; set; } = string.Empty;

        public string PublicBaseUrl { get; set; } = string.Empty;
    }

    public sealed class LocalStorageOptions
    {
        /// <summary>Thư mục con dưới ContentRoot (ví dụ wwwroot/uploads).</summary>
        public string RelativeRoot { get; set; } = "wwwroot/uploads";
    }
}
