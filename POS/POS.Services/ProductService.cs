using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Contracts.Repositories;
using POS.Domain.Contracts.Services;
using POS.Domain.Contracts.UnitOfWorks;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.Products;
using POS.Domain.Models.CQRS.Queries.Products;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;
using POS.Domain.Shared.Exceptions;
using POS.Domain.Utilities;
using POS.Services.Bases;
using Exception = System.Exception;

namespace POS.Services;

public class ProductService : BaseService, IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IMapper mapper,
        IUnitOfWork unitOfWork)
        : base(mapper, unitOfWork)
    {
        _productRepository = _unitOfWork.ProductRepository;
    }

    public async Task<BusinessResult> GetAll(ProductGetAllQuery query)
    {
        var queryable = _productRepository.GetQueryable();

        queryable = queryable.Include(query.IncludeProperties);
        queryable = queryable.Sort(query.Sorting);

        var pagedListProduct = await queryable.ToPagedListAsync(query.Pagination.PageNumber, query.Pagination.PageSize);
        var pagedList = _mapper.Map<IPagedList<ProductResult>>(pagedListProduct);


        return new BusinessResult(pagedList);
    }

    public Task<BusinessResult> Create(ProductCreateCommand createCommand)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResult> Update(ProductUpdateCommand updateCommand)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResult> GetById(ProductGetByIdQuery request)
    {
        var queryable = _productRepository.GetQueryable(x => x.Id == request.Id);
        queryable = RepoHelper.Include(queryable, request.IncludeProperties);
        var entity = await queryable.SingleOrDefaultAsync();
        if (entity == null) throw new NotFoundException("Not found");
        var result = _mapper.Map<ProductResult>(entity);

        return new BusinessResult(result);
    }

    public async Task<BusinessResult> Delete(ProductDeleteCommand command)
    {
        var entity = await _productRepository.GetQueryable(x => x.Id == command.Id).SingleOrDefaultAsync();
        if (entity == null) throw new NotFoundException(Const.NOT_FOUND_MSG);

        _productRepository.Delete(entity, command.IsPermanent);

        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
            throw new Exception();

        return new BusinessResult();
    }
}