using AutoMapper;
using Microsoft.AspNetCore.Http;
using POS.Domain.Contracts.Services.Bases;
using POS.Domain.Contracts.UnitOfWorks;

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