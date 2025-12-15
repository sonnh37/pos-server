using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Contracts.Services;
using POS.Domain.Contracts.UnitOfWorks;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.CQRS.Queries.Orders;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;
using POS.Domain.Shared.Exceptions;
using POS.Domain.Utilities;
using POS.Services.Bases;

namespace POS.Services;

public class OrderService : BaseService, IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _orderRepository = _unitOfWork.OrderRepository;
    }

    public async Task<BusinessResult> GetAll(OrderGetAllQuery query)
    {
        var queryable = _orderRepository.GetQueryable();

        queryable = queryable.Include(query.IncludeProperties);
        queryable = queryable.Sort(query.Sorting);

        var pagedListOrder = await queryable.ToPagedListAsync(query.Pagination.PageNumber, query.Pagination.PageSize);
        var pagedList = _mapper.Map<IPagedList<OrderResult>>(pagedListOrder);

        return new BusinessResult(pagedList);
    }

    public Task<BusinessResult> Create(OrderCreateCommand createCommand)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResult> Update(OrderUpdateCommand updateCommand)
    {
        throw new NotImplementedException();
    }


    public async Task<BusinessResult> CreateOrUpdate(CreateOrUpdateCommand createOrUpdateCommand)
    {
        Order? entity = null;
        if (createOrUpdateCommand is OrderUpdateCommand updateCommand)
        {
            entity = await _orderRepository.GetQueryable(m => m.Id == updateCommand.Id).SingleOrDefaultAsync();

            if (entity == null)
                throw new NotFoundException(Const.NOT_FOUND_MSG);

            _mapper.Map(updateCommand, entity);
            _orderRepository.Update(entity);
        }
        else if (createOrUpdateCommand is OrderCreateCommand createCommand)
        {
            entity = _mapper.Map<Order>(createCommand);
            if (entity == null) throw new NotFoundException(Const.NOT_FOUND_MSG);
            entity.CreatedDate = DateTimeOffset.UtcNow;
            _orderRepository.Add(entity);
        }

        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
            throw new Exception();

        var result = _mapper.Map<OrderResult>(entity);

        return new BusinessResult(result);
    }

    public async Task<BusinessResult> GetById(OrderGetByIdQuery request)
    {
        var queryable = _orderRepository.GetQueryable(x => x.Id == request.Id);
        queryable = RepoHelper.Include(queryable, request.IncludeProperties);
        var entity = await queryable.SingleOrDefaultAsync();
        if (entity == null) throw new NotFoundException("Not found");
        var result = _mapper.Map<OrderResult>(entity);

        return new BusinessResult(result);
    }

    public async Task<BusinessResult> Delete(OrderDeleteCommand command)
    {
        var entity = await _orderRepository.GetQueryable(x => x.Id == command.Id).SingleOrDefaultAsync();
        if (entity == null) throw new NotFoundException(Const.NOT_FOUND_MSG);

        _orderRepository.Delete(entity, command.IsPermanent);

        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
            throw new Exception();

        return new BusinessResult();
    }
}