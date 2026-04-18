using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SpaceShopper.API.Middlewares;
using SpaceShopper.Application.Common.Mappings;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Interfaces.IRepositories.Catalog;
using SpaceShopper.Application.Interfaces.IRepositories.Common;
using SpaceShopper.Application.Interfaces.IRepositories.Users;
using SpaceShopper.Application.Interfaces.Iservices.Catalog;
using SpaceShopper.Application.Interfaces.Iservices.Auth;
using SpaceShopper.Application.Interfaces.Iservices.Common;
using SpaceShopper.Application.Interfaces.Iservices.Users;
using SpaceShopper.Application.Interfaces.Security;
using SpaceShopper.Application.Services;
using SpaceShopper.Application.Services.Catalog;
using SpaceShopper.Application.Services.Auth;
using SpaceShopper.Application.Services.Users;
using SpaceShopper.Infrastructure.Caching.Extensions;
using SpaceShopper.Infrastructure.Data;
using SpaceShopper.Infrastructure.Repositories.Catalog;
using SpaceShopper.Infrastructure.Repositories.Common;
using SpaceShopper.Infrastructure.Repositories.Users;
using SpaceShopper.Infrastructure.Security;
using SpaceShopper.Infrastructure.Services.Email;


// using Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/spaceshopper-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddDbContext<SpaceShopperDbContext>(options => 
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "spaceshopper")
    )
);

builder.Services.AddRedisCaching(builder.Configuration);

// Authentication & Authorization (JWT)
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
                 ?? throw new InvalidOperationException("Jwt configuration section is missing or invalid.");

if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured.");
}

var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// DI for repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


// DI for services application
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICacheKeyHashService, CacheKeyHashService>();
builder.Services.AddScoped<IEmailService, MailKitEmailService>();

builder.Services.AddHttpClient<DevService>(client =>
{
    client.BaseAddress = new Uri("https://course.spacedev.vn");
});

builder.Services.AddTransient<GlobalExceptionMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
