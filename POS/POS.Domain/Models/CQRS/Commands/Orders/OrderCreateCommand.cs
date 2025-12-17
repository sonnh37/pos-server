using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;

namespace POS.Domain.Models.CQRS.Commands.Orders;

public class OrderCreateCommand : CreateCommand
{
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}