using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SpaceShopper.Infrastructure.Data
{
    public class SpaceShopperDbContextFactory : IDesignTimeDbContextFactory<SpaceShopperDbContext>
    {
        public SpaceShopperDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "../SpaceShopper.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("PostgresConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Missing 'ConnectionStrings:PostgresConnection'. " +
                    "Set it in appsettings or environment variable 'ConnectionStrings__PostgresConnection'.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<SpaceShopperDbContext>().UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly("SpaceShopper.Infrastructure");
                b.MigrationsHistoryTable("__EFMigrationsHistory", "spaceshopper");
            }).Options;

            return new SpaceShopperDbContext(optionsBuilder);

        }
    }
}
