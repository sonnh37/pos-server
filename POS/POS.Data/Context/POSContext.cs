using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Utilities;

namespace POS.Data.Context;

public partial class POSContext : BaseDbContext
{
    public POSContext(DbContextOptions<POSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    // Auto Enum Convert Int To String
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Model.GetEntityTypes()
            .ToList()
            .ForEach(e =>
            {
                e.SetTableName(NamingHelper.ToSnakeCase(e.DisplayName()));

                foreach (var p in e.GetProperties())
                    p.SetColumnName(NamingHelper.ToSnakeCase(p.Name));
            });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasMany(m => m.OrderItems)
                .WithOne(m => m.Product)
                .HasForeignKey(m => m.ProductId);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasMany(m => m.OrderItems)
                .WithOne(m => m.Order)
                .HasForeignKey(m => m.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}