using POS.Data.Context;
using POS.Data.Repositories.Base;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Entities;

namespace POS.Data.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(POSContext dbContext) : base(dbContext)
    {
    }
}