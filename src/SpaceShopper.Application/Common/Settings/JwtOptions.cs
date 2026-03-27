namespace SpaceShopper.Application.Common.Settings
{
    /// <summary>
    /// Cấu hình Jwt, bind từ section "Jwt" trong appsettings.json.
    /// </summary>
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Thời hạn access token (phút).
        /// Có thể dùng ở JwtService khi generate token.
        /// </summary>
        public TimeSpan AccessTokenExpiration { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Thời hạn refresh token (ngày).
        /// </summary>
        public TimeSpan RefreshTokenAbsoluteExpiration { get; set; } = TimeSpan.FromDays(7);

        /// <summary>
        /// Thời hạn refresh token (ngày).
        /// </summary>
        ///
        public TimeSpan RefreshTokenSlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);
    }
}

