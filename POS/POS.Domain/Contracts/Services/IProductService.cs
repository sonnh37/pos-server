using POS.Domain.Contracts.Services.Bases;
using POS.Domain.Models.CQRS.Commands.Base;
using POS.Domain.Models.CQRS.Commands.Products;
using POS.Domain.Models.CQRS.Queries.Products;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Contracts.Services;

public interface IProductService : IBaseService
{
    Task<BusinessResult> GetAll(ProductGetAllQuery query);
    Task<BusinessResult> Create(ProductCreateCommand createCommand);
    Task<BusinessResult> Update(ProductUpdateCommand updateCommand);
    Task<BusinessResult> GetById(ProductGetByIdQuery request);
    Task<BusinessResult> Delete(ProductDeleteCommand command);
}