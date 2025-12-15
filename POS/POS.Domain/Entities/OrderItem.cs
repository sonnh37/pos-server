using POS.Domain.Entities.Bases;

namespace POS.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
}