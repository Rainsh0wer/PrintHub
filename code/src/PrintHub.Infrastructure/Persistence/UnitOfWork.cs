using Microsoft.EntityFrameworkCore.Storage;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Domain.Common;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Wraps the DbContext as a single transactional boundary. Repositories are
/// cached per type so that all work within one request shares the same change
/// tracker and commits atomically through SaveChanges.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly PrintHubDbContext _db;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(PrintHubDbContext db) => _db = db;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        if (_repositories.TryGetValue(typeof(T), out var existing))
            return (IRepository<T>)existing;

        var repository = new Repository<T>(_db);
        _repositories[typeof(T)] = repository;
        return repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    // The DbContext itself is owned and disposed by the DI container (scoped);
    // the unit of work only owns the ambient transaction.
    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
