using Microsoft.EntityFrameworkCore;
using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Common;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// Translates an <see cref="ISpecification{T}"/> into an EF Core query — the
/// bridge that keeps specifications free of any EF dependency. Ordering is
/// applied before paging, as SQL requires.
/// </summary>
public static class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> input, ISpecification<T> spec)
    {
        var query = input;

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (spec.IgnoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (spec.OrderBy is not null)
        {
            var ordered = query.OrderBy(spec.OrderBy);
            if (spec.ThenBy is not null) ordered = ordered.ThenBy(spec.ThenBy);
            else if (spec.ThenByDescending is not null) ordered = ordered.ThenByDescending(spec.ThenByDescending);
            query = ordered;
        }
        else if (spec.OrderByDescending is not null)
        {
            var ordered = query.OrderByDescending(spec.OrderByDescending);
            if (spec.ThenBy is not null) ordered = ordered.ThenBy(spec.ThenBy);
            else if (spec.ThenByDescending is not null) ordered = ordered.ThenByDescending(spec.ThenByDescending);
            query = ordered;
        }

        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        if (spec.AsNoTracking)
            query = query.AsNoTracking();

        return query;
    }

    /// <summary>
    /// Applies only the parts of a specification that affect a COUNT — criteria
    /// and filter toggles — deliberately skipping includes, ordering, and paging
    /// so that a total count reflects the full filtered set.
    /// </summary>
    public static IQueryable<T> GetQueryForCount(IQueryable<T> input, ISpecification<T> spec)
    {
        var query = input;

        if (spec.IgnoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        return query;
    }
}
