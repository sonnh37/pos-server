using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;

namespace POS.Domain.Models.CQRS.Commands.Products;

public class ProductUpdateCommand : UpdateCommand
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
}