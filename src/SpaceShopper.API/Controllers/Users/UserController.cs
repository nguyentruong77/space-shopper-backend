using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Dtos.Users;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Requests.Users;

namespace SpaceShopper.API.Controllers.Users
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class UserController(
        IUserService userService,
        IAddressService addressService,
        IUserPaymentMethodService paymentMethodService,
        IWishlistService wishlistService) : BaseController
    {
        private readonly IUserService _userService = userService;
        private readonly IAddressService _addressService = addressService;
        private readonly IUserPaymentMethodService _paymentMethodService = paymentMethodService;
        private readonly IWishlistService _wishlistService = wishlistService;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            await _userService.RegisterAsync(request, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { message = "Register request accepted." }));
        }

        [HttpPost("resend-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmail([FromBody] ResendEmailRequest request, CancellationToken cancellationToken)
        {
            await _userService.ResendEmailAsync(request, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { message = "Verification email resent." }));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _userService.ResetPasswordAsync(request, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { message = "Reset email sent." }));
        }

        [HttpPost("change-password-by-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePasswordByCode([FromBody] ChangePasswordByCodeRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.ChangePasswordByCodeAsync(request, cancellationToken);
            return Ok(ApiResponse<AuthTokenResponse>.Ok(result));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            await _userService.ChangePasswordAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { message = "Password changed successfully." }));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
        {
            var user = await _userService.GetInfoAsync(GetCurrentUserId(), cancellationToken);
            return Ok(ApiResponse<UserInfoDto>.Ok(user));
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserInfoRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.UpdateInfoAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<UserInfoDto>.Ok(user));
        }

        [HttpGet("address")]
        [Authorize]
        public async Task<IActionResult> GetAddresses([FromQuery(Name = "default")] bool? isDefault, CancellationToken cancellationToken)
        {
            var addresses = await _addressService.GetAddressesAsync(GetCurrentUserId(), isDefault, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<UserAddressDto>>.Ok(addresses));
        }

        [HttpGet("address/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetAddressById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var address = await _addressService.GetAddressByIdAsync(GetCurrentUserId(), id, cancellationToken);
            return Ok(ApiResponse<UserAddressDto>.Ok(address));
        }

        [HttpPost("address")]
        [Authorize]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request, CancellationToken cancellationToken)
        {
            var address = await _addressService.AddAddressAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<UserAddressDto>.Ok(address));
        }

        [HttpPatch("address/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> EditAddress([FromRoute] Guid id, [FromBody] EditAddressRequest request, CancellationToken cancellationToken)
        {
            var address = await _addressService.EditAddressAsync(GetCurrentUserId(), id, request, cancellationToken);
            return Ok(ApiResponse<UserAddressDto>.Ok(address));
        }

        [HttpDelete("address/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveAddress([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _addressService.RemoveAddressAsync(GetCurrentUserId(), id, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        [HttpGet("payment")]
        [Authorize]
        public async Task<IActionResult> GetPayments(CancellationToken cancellationToken)
        {
            var payments = await _paymentMethodService.GetPaymentsAsync(GetCurrentUserId(), cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<UserPaymentDto>>.Ok(payments));
        }

        [HttpGet("payment/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var payment = await _paymentMethodService.GetPaymentByIdAsync(GetCurrentUserId(), id, cancellationToken);
            return Ok(ApiResponse<UserPaymentDto>.Ok(payment));
        }

        [HttpPost("payment")]
        [Authorize]
        public async Task<IActionResult> AddPayment([FromBody] AddPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _paymentMethodService.AddPaymentAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<UserPaymentDto>.Ok(payment));
        }

        [HttpPatch("payment/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> EditPayment([FromRoute] Guid id, [FromBody] EditPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _paymentMethodService.EditPaymentAsync(GetCurrentUserId(), id, request, cancellationToken);
            return Ok(ApiResponse<UserPaymentDto>.Ok(payment));
        }

        [HttpDelete("payment/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> RemovePayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _paymentMethodService.RemovePaymentAsync(GetCurrentUserId(), id, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { deleted = true }));
        }

        [HttpGet("wishlist")]
        [Authorize]
        public async Task<IActionResult> GetWishlist(CancellationToken cancellationToken)
        {
            var items = await _wishlistService.GetWishlistAsync(GetCurrentUserId(), cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<WishlistItemDto>>.Ok(items));
        }

        [HttpPost("wishlist/{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> AddToWishlist([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            var item = await _wishlistService.AddWishlistAsync(GetCurrentUserId(), productId, cancellationToken);
            return Ok(ApiResponse<WishlistItemDto>.Ok(item));
        }

        [HttpDelete("wishlist/{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            await _wishlistService.RemoveWishlistAsync(GetCurrentUserId(), productId, cancellationToken);
            return Ok(ApiResponse<object>.Ok(new { deleteCount = 1 }));
        }
    }
}
