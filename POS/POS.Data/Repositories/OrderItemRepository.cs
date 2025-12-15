using POS.Data.Context;
using POS.Data.Repositories.Base;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Entities;

namespace POS.Data.Repositories;

public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(POSContext dbContext) : base(dbContext)
    {
    }
}