using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;

namespace POS.Domain.Models.CQRS.Commands.Orders;

public class OrderUpdateCommand : UpdateCommand
{
    public string? OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset OrderDate { get; set; }
}