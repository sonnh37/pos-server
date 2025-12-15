using Microsoft.Extensions.Logging;
using POS.Data.Context;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Contracts.UnitOfWorks;

namespace POS.Data.UnitOfWorks;

public class UnitOfWork : BaseUnitOfWork<POSContext>, IUnitOfWork
{
    public UnitOfWork(POSContext context, IServiceProvider serviceProvider,
        ILogger<BaseUnitOfWork<POSContext>> logger)
        : base(context, serviceProvider, logger)
    {
    }

    public IProductRepository ProductRepository => GetRepository<IProductRepository>();

    public IOrderRepository OrderRepository => GetRepository<IOrderRepository>();

    public IOrderItemRepository OrderItemRepository => GetRepository<IOrderItemRepository>();
}