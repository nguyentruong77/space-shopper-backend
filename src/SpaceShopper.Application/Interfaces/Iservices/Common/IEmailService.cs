namespace SpaceShopper.Application.Interfaces.Iservices.Common
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
    }
}
