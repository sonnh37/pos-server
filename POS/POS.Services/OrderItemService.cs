using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Contracts.Services;
using POS.Domain.Contracts.UnitOfWorks;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.OrderItems;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;
using POS.Domain.Shared.Exceptions;
using POS.Domain.Utilities;
using POS.Services.Bases;

namespace POS.Services;

public class OrderItemService : BaseService, IOrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;

    public OrderItemService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _orderItemRepository = _unitOfWork.OrderItemRepository;
    }

    public Task<BusinessResult> Create(OrderItemCreateCommand createCommand)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResult> Update(OrderItemUpdateCommand updateCommand)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResult> Delete(OrderItemDeleteCommand command)
    {
        var entity = await _orderItemRepository.GetQueryable(x => x.Id == command.Id).SingleOrDefaultAsync();
        if (entity == null) throw new NotFoundException(Const.NOT_FOUND_MSG);

        _orderItemRepository.Delete(entity, command.IsPermanent);

        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
            throw new Exception();

        return new BusinessResult();
    }
}