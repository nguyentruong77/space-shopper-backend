namespace SpaceShopper.Application.Common.Email
{
    /// <summary>
    /// Chọn file HTML trong Infrastructure/Services/Email/Templates khi gửi mail.
    /// None = gửi body dạng text thuần (không đọc template).
    /// </summary>
    public enum EmailTemplateKind
    {
        None = 0,
        Verification = 1,
        ResendVerification = 2,
        PasswordReset = 3
    }
}
