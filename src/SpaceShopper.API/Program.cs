using Microsoft.EntityFrameworkCore;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Services;
using SpaceShopper.Application.Services.Catalog;
using SpaceShopper.Infrastructure.Persistence.Catalog;
using SpaceShopper.Infrastructure.Repositories.Catalog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<CatalogDbContext>(options => 
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection")
    )
);

// DI for repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();


// DI for services application
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpClient<DevService>(client =>
{
    client.BaseAddress = new Uri("https://course.spacedev.vn");
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
