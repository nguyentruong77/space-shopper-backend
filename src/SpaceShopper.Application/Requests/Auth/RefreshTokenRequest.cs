namespace SpaceShopper.Application.Requests.Auth
{
    public sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}

