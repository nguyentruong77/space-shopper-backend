using SpaceShopper.Application.Dtos.Catalog;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Requests.Catalog;

namespace SpaceShopper.Application.Interfaces.Iservices.Catalog
{
    public interface IProductService
    {
        Task<PagedResult<ProductListItemDto>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);
        Task<ProductDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
