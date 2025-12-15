using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Models.Results;

public class OrderItemResult : BaseResult
{
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderResult? Order { get; set; }
    public ProductResult? Product { get; set; }
}