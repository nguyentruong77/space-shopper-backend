namespace SpaceShopper.Application.Dtos.Users
{
    public sealed class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Fb { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; } = string.Empty;
    }

    public sealed class UserAddressDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public sealed class UserPaymentDto
    {
        public Guid Id { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string Expired { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
