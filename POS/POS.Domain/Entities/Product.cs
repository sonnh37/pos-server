using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities.Bases;

namespace POS.Domain.Entities;

public enum ProductStatus
{
    Available,
    OutOfStock,
    Discontinued,
    ComingSoon
}

public class Product : BaseEntity
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}