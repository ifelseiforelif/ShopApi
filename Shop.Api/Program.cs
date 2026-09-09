using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shop.Api.Interfaces;
using Shop.Api.Services;
using Shop.Application;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Queries.Product;
using Shop.Application.Services;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Seeds;
using Shop.Infrastructure.Services;
using StackExchange.Redis;
using System.Text;

namespace Shop.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ================= DATABASE =================

        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString(
                    "SqlServerConnection"));
        });


        // ================= JWT SETTINGS =================

        var configuration = builder.Configuration;

        var jwtSettings = configuration
            .GetSection("Jwt")
            .Get<JwtSettings>()
            ?? throw new Exception("JWT settings not configured.");

        builder.Services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));


        // ================= RABBIT MQ SETTINGS =================

        builder.Services.Configure<RabbitMqSettings>(
            configuration.GetSection("RabbitMq"));


        // ================= AUTOMAPPER =================

        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly,
            typeof(UserProfile).Assembly
        );


        // ================= CORS =================

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        //==================MEDIATR======================
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

        });

        // ================= CONTROLLERS =================

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();


        // ================= SWAGGER =================

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)
                    ] = []
                });
        });


        // ================= PROVIDERS =================

        builder.Services.AddScoped<
            IFilePathProvider,
            FilePathProvider
        >();


        // ================= REDIS =================

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = configuration
                .GetConnectionString("RedisServerConnection");

            if (string.IsNullOrWhiteSpace(config))
            {
                throw new InvalidOperationException(
                    "Redis connection string is not configured.");
            }

            return ConnectionMultiplexer.Connect(
                config,
                options =>
                {
                    options.AbortOnConnectFail = false;
                });
        });


        // ================= APPLICATION SERVICES =================

        builder.Services.AddScoped<
         IImageService,
         ImageService
     >();

        builder.Services.AddSingleton<
            IHashHelper,
            HashHelper
        >();

        builder.Services.AddScoped<
            ICategoryService,
            CategoryService
        >();

        builder.Services.AddScoped<
            IAuthService,
            AuthService
        >();

        builder.Services.AddScoped<
            IProductService,
            ProductService
        >();

     

        builder.Services.AddScoped<
            IJWTService,
            JWTService
        >();


        // ================= ADMIN SEEDER =================

        builder.Services.AddScoped<AdminSeeder>();


        // ================= CACHE =================

        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton<
            ICachingService,
            RedisCachingService
        >();


        // ================= RABBIT MQ =================

        builder.Services.AddHostedService<
            RabbitMqReaderService
        >();

        builder.Services.AddSingleton<
            IQueueService,
            RabbitMqService
        >();


        // ================= REPOSITORIES =================

        builder.Services.AddScoped<
            IProductRepository,
            ProductRepository
        >();

        builder.Services.AddScoped<
            ICategoryRepository,
            CategoryRepository
        >();

        builder.Services.AddScoped<
            IAuthRepository,
            AuthRepository
        >();

        builder.Services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository
        >();


        // ================= AUTHENTICATION =================

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.Key)
                            ),

                        ClockSkew = TimeSpan.Zero
                    };
            });


        // ================= AUTHORIZATION =================

        builder.Services.AddAuthorization();


        // ================= BUILD APP =================

        var app = builder.Build();


        // ================= DATABASE SEED =================

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider
                .GetRequiredService<AdminSeeder>();

            await seeder.SeedAsync();
        }


        // ================= MIDDLEWARE =================

        app.UseCors("AllowAll");


        // ================= SWAGGER =================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        // ================= HTTPS =================

        app.UseHttpsRedirection();


        // ================= AUTH =================

        app.UseAuthentication();
        app.UseAuthorization();


        // ================= STATIC FILES =================

        app.UseStaticFiles();


        // ================= CONTROLLERS =================

        app.MapControllers();


        // ================= RUN =================

        app.Run();
    }
}