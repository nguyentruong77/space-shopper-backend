using Microsoft.EntityFrameworkCore;
using SpaceShopper.Domain.Common;

namespace SpaceShopper.Infrastructure.Common.Extensions
{
    public static class DbContextExtension
    {
        public static IQueryable<T> Available<T>(this DbContext context, bool asNoTracking = true) where T : SoftDeletableAggregateRoot
        {
            var query = context.Set<T>().Where(e => !e.IsDeleted);
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }
    }
}
