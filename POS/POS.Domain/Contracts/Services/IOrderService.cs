using POS.Domain.Contracts.Services.Bases;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.CQRS.Queries.Orders;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Contracts.Services;

public interface IOrderService : IBaseService
{
    Task<BusinessResult> GetAll(OrderGetAllQuery query);
    Task<BusinessResult> Create(OrderCreateCommand createCommand);
    Task<BusinessResult> Update(OrderUpdateCommand updateCommand);
    Task<BusinessResult> GetById(OrderGetByIdQuery request);
    Task<BusinessResult> Delete(OrderDeleteCommand command);
}