using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Queries.Base;

namespace POS.Domain.Models.CQRS.Queries.Products;

public class ProductGetAllQuery : GetAllQuery
{
    public string? Name { get; set; }
    public ProductStatus? Status { get; set; }
}