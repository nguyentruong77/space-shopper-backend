namespace SpaceShopper.Infrastructure.Storage
{
    internal static class StoragePublicUrl
    {
        internal static string Combine(string? baseUrl, string objectKey)
        {
            var key = objectKey.TrimStart('/').Replace('\\', '/');
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return "/" + key;
            }

            return baseUrl.TrimEnd('/') + "/" + key;
        }
    }
}
