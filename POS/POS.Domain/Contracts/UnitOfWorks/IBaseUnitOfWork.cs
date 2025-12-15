using POS.Domain.Contracts.Repositories.Bases;
using POS.Domain.Entities.Bases;

namespace POS.Domain.Contracts.UnitOfWorks;

public interface IBaseUnitOfWork : IDisposable
{
    IBaseRepository<TEntity> GetRepositoryByEntity<TEntity>() where TEntity : BaseEntity;

    TRepository GetRepository<TRepository>() where TRepository : IBaseRepository;

    Task<bool> SaveChanges(CancellationToken cancellationToken = default);
}