using SpaceShopper.Domain.Entities.Promotions;

namespace SpaceShopper.Application.Interfaces.IRepositories.Promotions
{
    public interface IPromotionRepository
    {
        Task<IReadOnlyList<Promotion>> GetActiveByCodesAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default);
    }
}
