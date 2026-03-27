using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Domain.Common;
using SpaceShopper.Infrastructure.Common.Extensions;
using System.Linq.Expressions;

namespace SpaceShopper.Infrastructure.Common
{
    public abstract class Repository<T>(DbContext context) : IRepository<T> where T : SoftDeletableAggregateRoot
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task AddEntityAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Available<T>().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        //public async Task<T?> GetByIdWithIncludeAsync(
        //    Guid id,
        //    Func<IQueryable<T>, IQueryable<T>> include,
        //    CancellationToken cancellationToken = default)
        //{
        //    return await include(context.Available<T>()).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        //}

        //public async Task<TDto?> GetDtoByIdAsync<TDto>(Guid id, CancellationToken cancellationToken = default) where TDto : class
        //{
        //    return await GetDtoByIdAsync<TDto>(id, null, cancellationToken);
        //}

        //public async Task<TDto?> GetDtoByIdAsync<TDto>(
        //    Guid id,
        //    Expression<Func<T, bool>>? additionalCondition = null,
        //    CancellationToken cancellationToken = default) where TDto : class
        //{
        //    var query = context.Available<T>(false)
        //            .Where(p => p.Id == id);

        //    if (additionalCondition != null)
        //    {
        //        query = query.Where(additionalCondition);
        //    }

        //    return await query
        //        .FirstOrDefaultAsync(cancellationToken);
        //}
    }
}
