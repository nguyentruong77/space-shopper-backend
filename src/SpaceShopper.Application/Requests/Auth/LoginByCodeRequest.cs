namespace SpaceShopper.Application.Requests.Auth
{
    public sealed class LoginByCodeRequest
    {
        public string Code { get; set; } = string.Empty;
    }
}

