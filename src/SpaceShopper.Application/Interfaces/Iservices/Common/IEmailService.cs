using SpaceShopper.Application.Common.Email;

namespace SpaceShopper.Application.Interfaces.Iservices.Common
{
    public interface IEmailService
    {
        /// <param name="body">Nội dung text; khi dùng template sẽ chèn vào placeholder {{Content}} (đã HTML-encode).</param>
        Task SendAsync(
            string toEmail,
            string subject,
            string body,
            EmailTemplateKind templateKind = EmailTemplateKind.None,
            CancellationToken cancellationToken = default);
    }
}
