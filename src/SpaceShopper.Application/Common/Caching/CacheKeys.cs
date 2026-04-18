namespace SpaceShopper.Application.Common.Caching
{
    /// <summary>
    /// Chuẩn hóa key cache Redis. Pattern: [entity/context]:[loại]:[phần mở rộng].
    /// Dùng ":" làm separator; query dùng ToString() của request class để key ngắn, dễ debug.
    /// Ví dụ:
    ///   ForEntity&lt;Product&gt;(id)           → "product:entity:{guid}"
    ///   ForQuery&lt;Product, ProductSearchRequest&gt;(request) → "product:query:productsearchrequest:k:iphone|c:...|p:1|ps:20"
    ///   ForRefreshToken(userId)              → "user:{guid}:refresh_token"
    /// </summary>
    public class CacheKeys
    {
        private const string Separator = ":";

        public static string ForEntity<T>(Guid entityId) where T : class
            => $"{typeof(T).Name.ToLowerInvariant()}{Separator}entity{Separator}{entityId}";

        public static string ForDto<TEntity, T>(Guid dtoId) where T : class
        {
            var name = GetNameFromDto<T>();
            return $"{typeof(TEntity).Name.ToLowerInvariant()}{Separator}dto{Separator}{name}{Separator}{dtoId}";
        }

        private static string GetNameFromDto<T>() where T : class
        {
            return typeof(T).Name
                        .Replace("dto", "", StringComparison.OrdinalIgnoreCase)
                        .ToLowerInvariant();
        }

        public static string ForRefreshToken(Guid userId)
            => $"user{Separator}{userId}{Separator}refresh_token";

        /// <summary>
        /// Key lưu tạm thông tin đăng ký cho flow login-by-code.
        /// Pattern: "auth:register-code:{code}"
        /// </summary>
        public static string AuthRegisterCode(string code)
            => $"auth{Separator}register-code{Separator}{code}";

        // Backward-compatible alias. Prefer AuthRegisterCode.
        public static string ForLoginByCode(string code) => AuthRegisterCode(code);

        public static string ForQuery<TEntity, T>(T? queryParams = null) where T : class
        {
            var queryString = queryParams is null
                ? "all"
                : queryParams.ToString();

            return $"{typeof(TEntity).Name.ToLowerInvariant()}{Separator}query{Separator}{typeof(T).Name.ToLowerInvariant()}{Separator}{queryString}";
        }

        public static string CatalogProductsSearch(string queryHash)
            => $"catalog{Separator}products{Separator}search{Separator}{queryHash}";

        public static string CatalogProductDetail(Guid id)
            => $"catalog{Separator}products{Separator}{id}";

        public static string CatalogCategoriesAll()
            => $"catalog{Separator}categories{Separator}all";

        public static string CatalogCategoryDetail(Guid id)
            => $"catalog{Separator}categories{Separator}{id}";
    }
}
