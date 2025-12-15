using POS.Domain.Entities;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Models.Results;

public class ProductResult : BaseResult
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
    public ICollection<OrderItemResult> OrderItems { get; set; } = new List<OrderItemResult>();
}