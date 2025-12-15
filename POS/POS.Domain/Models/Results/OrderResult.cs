using POS.Domain.Entities;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Models.Results;

public class OrderResult : BaseResult
{
    public string? OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    
    public ICollection<OrderItemResult> OrderItems { get; set; } = new List<OrderItemResult>();
}