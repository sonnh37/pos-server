using AutoMapper;
using Microsoft.AspNetCore.Http;
using POS.Domain.Contracts.Services.Bases;
using POS.Domain.Contracts.UnitOfWorks;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;
using POS.Domain.Shared.Exceptions;
using POS.Domain.Utilities;

namespace POS.Services.Bases;

public abstract class BaseService : IBaseService
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IMapper _mapper;
    protected readonly IUnitOfWork _unitOfWork;

    protected BaseService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _httpContextAccessor ??= new HttpContextAccessor();
    }

}

public enum EntityOperation
{
    Create,
    Update
}