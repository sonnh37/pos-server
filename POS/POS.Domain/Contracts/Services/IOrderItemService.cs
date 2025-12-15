using POS.Domain.Contracts.Services.Bases;
using POS.Domain.Models.CQRS.Commands.OrderItems;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Contracts.Services;

public interface IOrderItemService : IBaseService
{
    Task<BusinessResult> Create(OrderItemCreateCommand createCommand);
    Task<BusinessResult> Update(OrderItemUpdateCommand updateCommand);
    Task<BusinessResult> Delete(OrderItemDeleteCommand command);
}