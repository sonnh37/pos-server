using System.Collections;
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

    public async Task<BusinessResult> Create(OrderCreateCommand createCommand)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in createCommand.Items)
        {
            var product = await _unitOfWork.ProductRepository.GetQueryable(m => m.Id == item.ProductId)
                .SingleOrDefaultAsync();
            if (product == null)
                throw new NotFoundException("Not found product: " + item.ProductId);
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                TotalAmount = product.Price * item.Quantity,
                CreatedDate = DateTimeOffset.UtcNow
            };
            orderItems.Add(orderItem);
        }

        var totalAmount = orderItems.Sum(o => o.TotalAmount);

        var order = new Order
        {
            OrderNumber = CommonHelper.GenerateId(),
            Status = OrderStatus.Pending,
            OrderDate =  DateTimeOffset.UtcNow,
            TotalAmount = totalAmount,
            OrderItems = orderItems
        };
        if (order == null) throw new NotFoundException(Const.NOT_FOUND_MSG);
        order.CreatedDate = DateTimeOffset.UtcNow;
        _orderRepository.Add(order);

        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
            throw new Exception();

        var result = _mapper.Map<OrderResult>(order);

        return new BusinessResult(result);
    }

    public async Task<BusinessResult> Update(OrderUpdateCommand updateCommand)
    {
        var entity = await _orderRepository.GetQueryable(m => m.Id == updateCommand.Id).SingleOrDefaultAsync();

        if (entity == null)
            throw new NotFoundException(Const.NOT_FOUND_MSG);

        _mapper.Map(updateCommand, entity);
        _orderRepository.Update(entity);

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