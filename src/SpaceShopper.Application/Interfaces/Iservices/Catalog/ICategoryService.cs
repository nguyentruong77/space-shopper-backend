using SpaceShopper.Application.Dtos.Catalog;

namespace SpaceShopper.Application.Interfaces.Iservices.Catalog
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
