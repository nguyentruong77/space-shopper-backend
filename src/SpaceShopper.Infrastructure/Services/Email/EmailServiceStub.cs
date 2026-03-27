using Microsoft.Extensions.Logging;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.Infrastructure.Services.Email
{
    public sealed class EmailServiceStub(ILogger<EmailServiceStub> logger) : IEmailService
    {
        private readonly ILogger<EmailServiceStub> _logger = logger;

        public Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Email stub sent to {ToEmail}. Subject: {Subject}.", toEmail, subject);
            return Task.CompletedTask;
        }
    }
}
