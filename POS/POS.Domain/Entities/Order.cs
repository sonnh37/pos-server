using POS.Domain.Entities.Bases;

namespace POS.Domain.Entities;

public class Order : BaseEntity
{
    public string? OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    ReadyForShipment,
    Shipped,
    Delivered,
    Completed,
    Cancelled,
    Refunded,
    OnHold
}