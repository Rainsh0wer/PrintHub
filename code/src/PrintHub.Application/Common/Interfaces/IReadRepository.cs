using PrintHub.Domain.Common;

namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Read-only escape hatch that exposes a composable <see cref="IQueryable{T}"/>.
/// Used by OData endpoints and AutoMapper ProjectTo, where the caller needs to
/// keep composing the query. Command-side code uses <see cref="IRepository{T}"/>
/// with specifications instead.
/// </summary>
public interface IReadRepository<T> where T : BaseEntity
{
    /// <summary>A no-tracking queryable with global query filters (e.g. soft-delete) applied.</summary>
    IQueryable<T> Query();
}
