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

        public async Task<List<Product>> GetListProductByQueryAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
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

            if (request.FilterRating.HasValue)
            {
                query = query.Where(p => p.RatingAverage >= request.FilterRating.Value);
            }

            query = request.Sort switch
            {
                ProductSort.PriceAsc => query.OrderBy(p => p.RealPrice),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.RealPrice),
                ProductSort.DiscountDesc => query.OrderByDescending(p => p.DiscountRate),
                ProductSort.RatingDesc => query.OrderByDescending(p => p.RatingAverage),
                _ => query.OrderByDescending(p => p.CreatedOn)
            };

            if (request.PageIndex > 0 && request.PageSize > 0)
            {
                var skip = (request.PageIndex - 1) * request.PageSize;
                query = query.Skip(skip).Take(request.PageSize);
            }
            else
            {
                query = query.Take(15);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
