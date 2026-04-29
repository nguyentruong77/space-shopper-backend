using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.Application.Interfaces.Iservices.Catalog
{
    public interface IReviewService
    {
        Task<ProductReviewListDto> GetReviewsByProductAsync(GetReviewsByProductRequest request, CancellationToken cancellationToken = default);
        Task<AddProductReviewResultDto> AddReviewAsync(Guid userId, AddReviewRequest request, CancellationToken cancellationToken = default);
    }
}
