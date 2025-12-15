using System.Reflection;
using System.Runtime.Loader;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Data.Context;
using POS.Data.Repositories;
using POS.Data.UnitOfWorks;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Contracts.Services;
using POS.Domain.Contracts.UnitOfWorks;
using POS.Domain.Models.Options;
using POS.Domain.Utilities;
using POS.Services;

namespace POS.API.Extensions;

public static class ServiceExtensions
{
    public static readonly string CorsPolicyName = "CorsPolicy";

    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            var origins = configuration.GetValue<string>("AllowOrigins")?.Split(",") ?? Array.Empty<string>();
            options.AddPolicy(CorsPolicyName, builder =>
            {
                if (origins.Length == 0)
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                else
                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
            });
        });
    }

    public static void ConfigureSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(nameof(UserJwtOptions)).Get<UserJwtOptions>();

                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = "Role",
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtOptions!.ValidIssuer,
                    ValidAudience = jwtOptions.ValidAudience,
                    IssuerSigningKey = new RsaSecurityKey(RsaHelper.CreateRsaFromPublicKey(jwtOptions.PublicKey)),
                };
            });

        services.AddAuthorization();
    }

   

    public static void ConfigureContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<POSContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    public static void ConfigureBind(this IServiceCollection services)
    {
        services.AddOptions<EmailOptions>()
            .BindConfiguration(nameof(EmailOptions));
        services.AddOptions<UserJwtOptions>()
            .BindConfiguration(nameof(UserJwtOptions));
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
    }

    public static void LoadAssemblies(this IServiceCollection services)
    {
        var folder = AppContext.BaseDirectory;
        foreach (var dll in Directory.GetFiles(folder, "*.dll"))
        {
            AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
        }
    }

    private static Assembly[] GetAssemblies()
    {
        return AssemblyLoadContext.Default.Assemblies
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
            .ToArray();
    }
}