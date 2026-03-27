namespace SpaceShopper.Application.Dtos.Auth
{
    public sealed class PendingUserRegister
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
    }
}

