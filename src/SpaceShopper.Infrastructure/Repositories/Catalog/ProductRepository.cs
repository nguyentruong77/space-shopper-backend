using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Requests.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Common.Extensions;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Catalog
{
    public class ProductRepository(SpaceShopperDbContext context) : Repository<Product>(context), IProductRepository
    {
        public async Task<bool> IsExistAsync(long id, CancellationToken cancellationToken = default)
        {
            return await context.Products.AsNoTracking().AnyAsync(e => e.IdClone == id, cancellationToken);
        }

        public async Task<Product?> GetDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Products
                .AsNoTracking()
                .Include(p => p.ProductImages)
                .Include(p => p.ProductStock)
                .Include(p => p.ProductReviews)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<Product> Items, int TotalItems)> GetListProductByQueryAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = context.Products
                .AsNoTracking()
                .Include(p => p.ProductImages)
                .Include(p => p.ProductStock)
                .Include(p => p.ProductReviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword)));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.RealPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.RealPrice <= request.MaxPrice.Value);
            }

            if (request.Rating.HasValue)
            {
                query = query.Where(p => p.RatingAverage >= (decimal)request.Rating.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            query = request.NormalizeSortValue() switch
            {
                "price_asc" => query.OrderBy(p => p.RealPrice),
                "price_desc" => query.OrderByDescending(p => p.RealPrice),
                "rating_desc" => query.OrderByDescending(p => p.RatingAverage),
                _ => query.OrderByDescending(p => p.CreatedOn)
            };

            if (request.Page > 0 && request.PageSize > 0)
            {
                var skip = (request.Page - 1) * request.PageSize;
                query = query.Skip(skip).Take(request.PageSize);
            }
            else
            {
                query = query.Take(15);
            }

            var items = await query.ToListAsync(cancellationToken);
            return (items, totalItems);
        }

        public async Task<IReadOnlyList<Product>> GetByIdsForWishlistAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
        {
            if (ids.Count == 0)
            {
                return Array.Empty<Product>();
            }

            return await context.Available<Product>()
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByIdsForCartAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
        {
            if (ids.Count == 0)
            {
                return Array.Empty<Product>();
            }

            return await context.Available<Product>()
                .Include(p => p.ProductStock)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByIdsForCheckoutAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
        {
            if (ids.Count == 0)
            {
                return Array.Empty<Product>();
            }

            return await context.Available<Product>(false)
                .Include(p => p.ProductStock)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdWithReviewsAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            var query = context.Available<Product>(asNoTracking)
                .Include(p => p.ProductReviews)
                .Where(p => p.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
