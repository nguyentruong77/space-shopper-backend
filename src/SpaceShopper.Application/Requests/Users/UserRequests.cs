using System.Text.Json.Serialization;

namespace SpaceShopper.Application.Requests.Users
{
    public sealed class RegisterRequest
    {
        public string? Email { get; set; }
        [JsonPropertyName("username")]
        public string? UsernameAlias { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Redirect { get; set; } = string.Empty;
    }

    public sealed class ResendEmailRequest
    {
        public string? Email { get; set; }
        [JsonPropertyName("username")]
        public string? UsernameAlias { get; set; }
    }

    public sealed class ResetPasswordRequest
    {
        public string? Email { get; set; }
        [JsonPropertyName("username")]
        public string? UsernameAlias { get; set; }
        public string Redirect { get; set; } = string.Empty;
    }

    public sealed class ChangePasswordByCodeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public sealed class UpdateUserInfoRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Fb { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Gender { get; set; }
    }

    public sealed class AddAddressRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public sealed class EditAddressRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public bool? IsDefault { get; set; }
    }

    public sealed class AddPaymentRequest
    {
        public string CardName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string Expired { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public sealed class EditPaymentRequest
    {
        public string? CardName { get; set; }
        public string? CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? Expired { get; set; }
        public string? Type { get; set; }
        public bool? IsDefault { get; set; }
    }
}
