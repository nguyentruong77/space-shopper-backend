namespace SpaceShopper.Application.Common.Settings
{
    /// <summary>
    /// Cấu hình SMTP, bind từ section "Smtp" trong appsettings; inject qua IOptions&lt;SmtpOptions&gt;.
    /// </summary>
    public sealed class SmtpOptions
    {
        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string FromEmail { get; set; } = string.Empty;

        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Cách bảo mật khi kết nối: StartTls (mặc định, cổng 587), SslOnConnect (465), None.
        /// </summary>
        public string SecureSocketOption { get; set; } = "StartTls";
    }
}
