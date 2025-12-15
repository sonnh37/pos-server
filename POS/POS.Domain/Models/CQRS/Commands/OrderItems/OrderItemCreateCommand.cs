using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;

namespace POS.Domain.Models.CQRS.Commands.OrderItems;

public class OrderItemCreateCommand : CreateCommand
{
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
   
}