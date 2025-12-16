using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SpaceShopper.Infrastructure.Persistence.Catalog
{
    public class CatalogContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SpaceShopper.API")).AddJsonFile("appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>().UseNpgsql(config.GetConnectionString("PostgresConnection"), b => b.MigrationsAssembly("SpaceShopper.Infrastructure")).Options;

            return new CatalogDbContext(optionsBuilder);

        }
    }
}
