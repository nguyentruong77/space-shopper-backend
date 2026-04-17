using System.Net;
using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SpaceShopper.Application.Common.Email;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Services.Email
{
    public sealed class MailKitEmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<MailKitEmailService> _logger;

        public MailKitEmailService(IOptions<SmtpOptions> options, ILogger<MailKitEmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(
            string toEmail,
            string subject,
            string body,
            EmailTemplateKind templateKind = EmailTemplateKind.None,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Host))
            {
                throw new InvalidOperationException("SMTP Host is not configured (Smtp:Host).");
            }

            if (string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                throw new InvalidOperationException("SMTP FromEmail is not configured (Smtp:FromEmail).");
            }

            var message = new MimeMessage();
            var displayName = string.IsNullOrWhiteSpace(_options.FromName) ? _options.FromEmail : _options.FromName;
            message.From.Add(new MailboxAddress(displayName, _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            if (templateKind == EmailTemplateKind.None)
            {
                message.Body = new TextPart("plain") { Text = body };
            }
            else
            {
                var html = await LoadTemplateHtmlAsync(templateKind, cancellationToken).ConfigureAwait(false);
                var safeContent = WebUtility.HtmlEncode(body);
                html = html.Replace("{{Content}}", safeContent, StringComparison.Ordinal);

                var builder = new BodyBuilder
                {
                    TextBody = body,
                    HtmlBody = html
                };
                message.Body = builder.ToMessageBody();
            }

            using var client = new SmtpClient();

            try
            {
                var secure = ParseSecureSocketOption(_options.SecureSocketOption);
                await client.ConnectAsync(_options.Host, _options.Port, secure, cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(_options.UserName))
                {
                    await client
                        .AuthenticateAsync(_options.UserName, _options.Password ?? string.Empty, cancellationToken)
                        .ConfigureAwait(false);
                }

                await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
                await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("Email sent to {ToEmail}. Subject: {Subject}.", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}. Subject: {Subject}.", toEmail, subject);
                throw;
            }
        }

        private static async Task<string> LoadTemplateHtmlAsync(EmailTemplateKind kind, CancellationToken cancellationToken)
        {
            var fileName = kind switch
            {
                EmailTemplateKind.Verification or EmailTemplateKind.ResendVerification => "verification.html",
                EmailTemplateKind.PasswordReset => "password-reset.html",
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Use EmailTemplateKind.None for plain text.")
            };

            var assembly = typeof(MailKitEmailService).Assembly;
            using var stream = FindTemplateStream(assembly, fileName);
            if (stream is null)
            {
                var available = string.Join(", ", assembly.GetManifestResourceNames());
                throw new InvalidOperationException($"Email template for '{fileName}' not found. Resources: {available}");
            }

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }

        private static Stream? FindTemplateStream(Assembly assembly, string fileName)
        {
            var suffix = $".Templates.{fileName}";
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(suffix, StringComparison.Ordinal))
                {
                    return assembly.GetManifestResourceStream(name);
                }
            }

            return null;
        }

        private static SecureSocketOptions ParseSecureSocketOption(string? value)
        {
            return value?.Trim().ToLowerInvariant() switch
            {
                "ssl" or "sslonconnect" => SecureSocketOptions.SslOnConnect,
                "none" or "plain" => SecureSocketOptions.None,
                _ => SecureSocketOptions.StartTls
            };
        }
    }
}
