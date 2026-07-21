using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Common;

namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Generic repository. Query logic is expressed through specifications rather
/// than through a proliferation of custom methods, keeping the repository thin.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}
