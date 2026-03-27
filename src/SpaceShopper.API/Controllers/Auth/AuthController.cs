using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Auth;
using SpaceShopper.Application.Interfaces.Iservices.Auth;
using SpaceShopper.Application.Requests.Auth;

namespace SpaceShopper.API.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(ApiResponse<AuthTokenResponse>.Ok(result));
        }

        [HttpPost("login-by-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginByCode([FromBody] LoginByCodeRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginByCodeAsync(request, cancellationToken);
            return Ok(ApiResponse<AuthTokenResponse>.Ok(result));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);
            return Ok(ApiResponse<AuthTokenResponse>.Ok(result));
        }
    }
}

