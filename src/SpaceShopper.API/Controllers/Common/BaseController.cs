using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;

namespace SpaceShopper.API.Controllers.Common
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                throw new UnauthorizedException(ErrorCodes.Auth.Unauthorized, ErrorMessages.Auth.Unauthorized);
            }

            return userId;
        }

        protected bool TryGetCurrentUserId(out Guid userId)
        {
            var rawUserId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(rawUserId, out userId);
        }
    }
}
