using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Requests.Catalog;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Infrastructure.Common;
using SpaceShopper.Infrastructure.Data;

namespace SpaceShopper.Infrastructure.Repositories.Catalog
{
    public class ProductRepository(SpaceShopperDbContext context) : Repository<Product>(context), IProductRepository
    {
        public async Task<bool> IsExistAsync(long id, CancellationToken cancellationToken = default)
        {
            return await context.Products.AsNoTracking().AnyAsync(e => e.IdClone == id, cancellationToken);
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
    }
}
