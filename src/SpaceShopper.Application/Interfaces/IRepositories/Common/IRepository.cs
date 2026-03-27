using SpaceShopper.Domain.Common;
using System.Linq.Expressions;

namespace SpaceShopper.Application.Interfaces.IRepositories.Common
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task AddEntityAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        //Task<T?> GetByIdWithIncludeAsync(Guid id, Func<IQueryable<T>, IQueryable<T>> include, CancellationToken cancellationToken = default);
        //Task<TDto?> GetDtoByIdAsync<TDto>(Guid id, CancellationToken cancellationToken = default) where TDto : class;
        //Task<TDto?> GetDtoByIdAsync<TDto>(Guid id, Expression<Func<T, bool>>? additionalCondition = null, CancellationToken cancellationToken = default) where TDto : class;
    }
}
