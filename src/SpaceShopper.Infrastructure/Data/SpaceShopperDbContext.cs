using Microsoft.EntityFrameworkCore;
using SpaceShopper.Domain.Entities.Catalog;
using SpaceShopper.Domain.Entities.Contents;
using SpaceShopper.Domain.Entities.Orders;
using SpaceShopper.Domain.Entities.Users;

namespace SpaceShopper.Infrastructure.Data
{
    public class SpaceShopperDbContext(DbContextOptions<SpaceShopperDbContext> options) : DbContext(options)
    {
        // Catalog entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        // Orders entities
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderPromotion> OrderPromotions { get; set; }
        public DbSet<OrderShipping> OrderShippings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        // Users entities
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }

        // Contents entities
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("spaceshopper");

            #region Catalog configuration
            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasKey(e => e.Id);

                // Foreign key relationship
                entity.HasMany(e => e.Products)
                      .WithOne(e => e.Category)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasKey(e => e.Id);

                // Foreign key relationship
                entity.HasMany(e => e.ProductImages).WithOne()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProductStock).WithOne()
                      .HasForeignKey<ProductStock>(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.ProductReviews).WithOne()
                        .HasForeignKey(e => e.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductImage entity configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");
                entity.HasKey(e => e.Id);
            });

            // ProductStock entity configuration
            modelBuilder.Entity<ProductStock>(entity =>
            {
                entity.ToTable("ProductStock");
                entity.HasKey(e => e.ProductId);
            });

            // ProductReview entity configuration
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.ToTable("ProductReview");
                entity.HasKey(e => e.Id);
            });
            #endregion

            #region Orders configuration
            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasKey(e => e.Id);

                // Foreign key relationship
                entity.HasMany(e => e.OrderDetails).WithOne()
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.OrderPromotion).WithOne()
                        .HasForeignKey<OrderPromotion>(e => e.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.OrderShipping).WithOne()
                        .HasForeignKey<OrderShipping>(e => e.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderDetail entity configuration
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");
                entity.HasKey(e => new { e.ProductId, e.OrderId });

                // Foreign key relationship

            });

            // OrderPromotion entity configuration
            modelBuilder.Entity<OrderPromotion>(entity =>
            {
                entity.ToTable("OrderPromotion");
                entity.HasKey(e => e.OrderId);
            });

            // OrderShipping entity configuration
            modelBuilder.Entity<OrderShipping>(entity =>
            {
                entity.ToTable("OrderShipping");
                entity.HasKey(e => e.OrderId);
            });

            // Promotion entity configuration
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("Promotion");
                entity.HasKey(e => e.Id);
            });
            #endregion

            #region Users configuration
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.Id);

                // Foreign key relationships
                entity.HasMany(e => e.UserCarts).WithOne()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserAddresses).WithOne()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserPaymentMethods).WithOne()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Orders).WithOne()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.WishlistItems).WithOne()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UserAddress entity configuration
            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("UserAddress");
                entity.HasKey(e => e.Id);
            });

            // UserCart entity configuration
            modelBuilder.Entity<UserCart>(entity =>
            {
                entity.ToTable("UserCart");
                entity.HasKey(e => new { e.UserId, e.ProductId });
            });

            // UserPaymentMethod entity configuration
            modelBuilder.Entity<UserPaymentMethod>(entity =>
            {
                entity.ToTable("UserPaymentMethod");
                entity.HasKey(e => e.Id);
            });

            // UserToken entity configuration
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserToken");
                entity.HasKey(e => e.UserId);
            });

            // WishlistItem entity configuration
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.ToTable("WishlistItem");
                entity.HasKey(e => new { e.UserId, e.ProductId });
            });
            #endregion

            #region Contents configuration
            // Contact entity configuration
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");
                entity.HasKey(e => e.Id);
            });
            #endregion
        }
    }
}
