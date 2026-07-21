using Microsoft.EntityFrameworkCore;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Common;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Generic repository over the DbContext. All non-trivial querying flows through
/// specifications, so this class stays thin and the same implementation serves
/// every entity. Also implements the read-only queryable escape hatch used by OData.
/// </summary>
public class Repository<T> : IRepository<T>, IReadRepository<T> where T : BaseEntity
{
    private readonly PrintHubDbContext _db;

    public Repository(PrintHubDbContext db) => _db = db;

    public IQueryable<T> Query() => _db.Set<T>().AsNoTracking();

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync(new object[] { id }, ct);

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).ToListAsync(ct);

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken ct = default)
        => await _db.Set<T>().ToListAsync(ct);

    public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpecForCount(spec).CountAsync(ct);

    public async Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpecForCount(spec).AnyAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _db.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => _db.Set<T>().Update(entity);

    public void Remove(T entity) => _db.Set<T>().Remove(entity);

    private IQueryable<T> ApplySpec(ISpecification<T> spec)
        => SpecificationEvaluator<T>.GetQuery(_db.Set<T>().AsQueryable(), spec);

    private IQueryable<T> ApplySpecForCount(ISpecification<T> spec)
        => SpecificationEvaluator<T>.GetQueryForCount(_db.Set<T>().AsQueryable(), spec);
}
