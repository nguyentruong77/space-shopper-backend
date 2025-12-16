using Microsoft.EntityFrameworkCore;
using SpaceShopper.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShopper.Infrastructure.Persistence.Catalog
{
    public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<ProductImage> ProductImages { get; set; }
        //public DbSet<ProductStock> ProductStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("catalog");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasKey(e => e.id);

                entity.OwnsOne(p => p.Price, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("price_amount");
                    money.Property(m => m.Currency).HasColumnName("price_currency");
                });
                entity.OwnsOne(p => p.RealPrice, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("real_price_amount");
                    money.Property(m => m.Currency).HasColumnName("real_price_currency");
                });
            });
        }
    }
}
