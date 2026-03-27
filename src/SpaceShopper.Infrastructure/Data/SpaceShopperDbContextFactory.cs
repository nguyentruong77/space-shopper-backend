using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SpaceShopper.Infrastructure.Data
{
    public class SpaceShopperDbContextFactory : IDesignTimeDbContextFactory<SpaceShopperDbContext>
    {
        public SpaceShopperDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SpaceShopper.API")).AddJsonFile("appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<SpaceShopperDbContext>().UseNpgsql(config.GetConnectionString("PostgresConnection"), b =>
            {
                b.MigrationsAssembly("SpaceShopper.Infrastructure");
                b.MigrationsHistoryTable("__EFMigrationsHistory", "spaceshopper");
            }).Options;

            return new SpaceShopperDbContext(optionsBuilder);

        }
    }
}
