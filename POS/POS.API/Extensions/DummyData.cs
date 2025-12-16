using Bogus;
using Microsoft.EntityFrameworkCore;
using POS.Data.Context;
using POS.Domain.Entities;
using POS.Domain.Models.Options;

namespace POS.API.Extensions;

public static class DummyData
{
     public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var seedOptions = configuration.GetSection(nameof(SeedOptions)).Get<SeedOptions>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            // Kiểm tra flag
            if (!seedOptions?.EnableSeeding ?? false)
            {
                logger.LogInformation("Seeding is disabled in configuration.");
                return app;
            }
            
            var context = services.GetRequiredService<POSContext>();

            logger.LogInformation("Starting database seeding...");
            
            // Xóa database nếu được cấu hình
            if (seedOptions?.DeleteDatabaseBeforeSeeding ?? false)
            {
                logger.LogWarning("Deleting database before seeding...");
                ClearAllEntities(context, logger);
            }
            
            // Seed data với cấu hình số lượng
            GenerateProducts(context, seedOptions.SeedDataCount.Products);
            // ... seed các bảng khác
            
            logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
        
        return app;
    }

    private static void ClearAllEntities(POSContext context, ILogger logger)
    {
        logger.LogInformation("Clearing all entities...");
    
        // Xóa theo thứ tự để tránh lỗi foreign key
        context.Orders.RemoveRange(context.Orders);
        context.Products.RemoveRange(context.Products);
        context.OrderItems.RemoveRange(context.OrderItems);
        // ... thêm các bảng khác
    
        context.SaveChanges();
    
        logger.LogInformation("All entities cleared successfully.");
    }
    private static void GenerateProducts(DbContext context, int count)
    {
        if (!context.Set<Product>().Any())
        {
            var productFaker = new Faker<Product>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Commerce.Product())
                .RuleFor(o => o.Price, f => f.Finance.Amount(10000, 999999))
                .RuleFor(c => c.Status, f => f.PickRandom<ProductStatus>())
                .RuleFor(o => o.CreatedDate, DateTime.UtcNow)
                .RuleFor(o => o.IsDeleted, f => false);

            var products = productFaker.Generate(count);

            context.Set<Product>().AddRange(products);
            context.SaveChanges();
        }
    }

}