using SpaceShopper.Domain.Common;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Domain.Enums;
using System.Linq;

namespace SpaceShopper.Domain.Entities.Users
{
    public class User : SoftDeletableAggregateRoot
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Fb { get; set; }
        public string PasswordHash { get; set; }
        public DateOnly BirthDay { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public ICollection<UserPaymentMethod> UserPaymentMethods { get; set; } = new List<UserPaymentMethod>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

        public int TokenVersion { get; set; }

        public UserToken AddToken(string refreshToken, DateTime expiresAt)
        {
            var token = new UserToken
            {
                UserId = Id,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                TokenVersion = this.TokenVersion
            };

            UserTokens.Add(token);
            return token;
        }

        public void RevokeToken(UserToken token)
        {
            if (UserTokens.Contains(token))
            {
                UserTokens.Remove(token);
            }
        }

        public UserToken? FindValidToken(string refreshToken, DateTime now)
        {
            return UserTokens.FirstOrDefault(t =>
                t.RefreshToken == refreshToken &&
                t.TokenVersion == TokenVersion &&
                !t.IsExpired(now));
        }

        public void UpdateProfile(string? name, string? phone, string? avatar, string? fb, DateOnly? birthDay, string? gender)
        {
            if (name is not null) Name = name;
            if (phone is not null) Phone = phone;
            if (avatar is not null) Avatar = avatar;
            if (fb is not null) Fb = fb;
            if (birthDay.HasValue) BirthDay = birthDay.Value;
            if (gender is not null) Gender = gender;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void IncrementTokenVersion()
        {
            TokenVersion++;
        }

        public UserAddress AddAddress(UserAddress address, bool isDefault = false)
        {
            address.UserId = Id;
            if (isDefault || !UserAddresses.Any())
            {
                SetDefaultAddressInternal(address.Id);
                address.Default = true;
            }

            UserAddresses.Add(address);
            return address;
        }

        public UserAddress UpdateAddress(Guid addressId, string? fullName, string? email, string? phone, string? province, string? district, string? address, bool? isDefault)
        {
            var existing = UserAddresses.FirstOrDefault(x => x.Id == addressId)
                ?? throw new KeyNotFoundException("Address was not found.");

            if (fullName is not null) existing.FullName = fullName;
            if (email is not null) existing.Email = email;
            if (phone is not null) existing.Phone = phone;
            if (province is not null) existing.Province = province;
            if (district is not null) existing.District = district;
            if (address is not null) existing.Address = address;
            if (isDefault.HasValue && isDefault.Value) SetDefaultAddress(addressId);

            return existing;
        }

        public void RemoveAddress(Guid addressId)
        {
            var existing = UserAddresses.FirstOrDefault(x => x.Id == addressId)
                ?? throw new KeyNotFoundException("Address was not found.");

            if (existing.Default)
            {
                throw new InvalidOperationException("Default address cannot be deleted.");
            }

            UserAddresses.Remove(existing);
        }

        public void SetDefaultAddress(Guid addressId)
        {
            if (!UserAddresses.Any(x => x.Id == addressId))
            {
                throw new KeyNotFoundException("Address was not found.");
            }

            SetDefaultAddressInternal(addressId);
        }

        public UserPaymentMethod AddPaymentMethod(UserPaymentMethod payment, bool isDefault = false)
        {
            payment.UserId = Id;
            if (isDefault || !UserPaymentMethods.Any())
            {
                SetDefaultPaymentMethodInternal(payment.Id);
                payment.Default = true;
            }

            UserPaymentMethods.Add(payment);
            return payment;
        }

        public UserPaymentMethod UpdatePaymentMethod(Guid paymentId, string? cardName, string? cardNumber, string? expired, string? type, bool? isDefault)
        {
            var existing = UserPaymentMethods.FirstOrDefault(x => x.Id == paymentId)
                ?? throw new KeyNotFoundException("Payment method was not found.");

            if (cardName is not null) existing.CardName = cardName;
            if (cardNumber is not null) existing.CardNumber = cardNumber;
            if (expired is not null) existing.ExpirationDate = expired;
            if (type is not null) existing.Type = type;
            if (isDefault.HasValue && isDefault.Value) SetDefaultPaymentMethod(paymentId);

            return existing;
        }

        public void RemovePaymentMethod(Guid paymentId)
        {
            var existing = UserPaymentMethods.FirstOrDefault(x => x.Id == paymentId)
                ?? throw new KeyNotFoundException("Payment method was not found.");

            if (existing.Default)
            {
                throw new InvalidOperationException("Default payment method cannot be deleted.");
            }

            UserPaymentMethods.Remove(existing);
        }

        public void SetDefaultPaymentMethod(Guid paymentId)
        {
            if (!UserPaymentMethods.Any(x => x.Id == paymentId))
            {
                throw new KeyNotFoundException("Payment method was not found.");
            }

            SetDefaultPaymentMethodInternal(paymentId);
        }

        private void SetDefaultAddressInternal(Guid addressId)
        {
            foreach (var item in UserAddresses)
            {
                item.Default = item.Id == addressId;
            }
        }

        private void SetDefaultPaymentMethodInternal(Guid paymentId)
        {
            foreach (var item in UserPaymentMethods)
            {
                item.Default = item.Id == paymentId;
            }
        }

        public WishlistItem AddToWishlist(Guid productId)
        {
            if (WishlistItems.Any(w => w.ProductId == productId))
            {
                throw new InvalidOperationException("Product is already in the wishlist.");
            }

            var item = new WishlistItem
            {
                UserId = Id,
                ProductId = productId
            };

            WishlistItems.Add(item);
            return item;
        }

        public void RemoveFromWishlist(Guid productId)
        {
            var existing = WishlistItems.FirstOrDefault(w => w.ProductId == productId)
                ?? throw new KeyNotFoundException("Wishlist item was not found.");

            WishlistItems.Remove(existing);
        }
    }
}
