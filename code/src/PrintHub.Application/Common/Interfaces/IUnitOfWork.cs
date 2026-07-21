using PrintHub.Domain.Common;

namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Unit of Work: a single transactional boundary over the repositories. Business
/// operations that touch multiple entities (place order + debit wallet + write
/// history) commit or roll back atomically through one SaveChanges.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
