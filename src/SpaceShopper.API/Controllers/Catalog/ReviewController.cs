using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.API.Controllers.Catalog
{
    [ApiController]
    [Route("api/v1/reviews")]
    public sealed class ReviewController(IReviewService reviewService) : BaseController
    {
        private readonly IReviewService _reviewService = reviewService;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByProduct([FromQuery] GetReviewsByProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _reviewService.GetReviewsByProductAsync(request, cancellationToken);
            return Ok(ApiResponse<ProductReviewListDto>.Ok(result));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AddProductReviewResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _reviewService.AddReviewAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(ApiResponse<AddProductReviewResultDto>.Ok(result));
        }
    }
}
