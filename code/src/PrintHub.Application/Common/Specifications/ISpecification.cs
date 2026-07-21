using System.Linq.Expressions;

namespace PrintHub.Application.Common.Specifications;

/// <summary>
/// Encapsulates a query — filtering, eager loading, ordering, and paging — as a
/// reusable, testable object. This is how getlist / search / filter / sort /
/// pagination is expressed across the platform: services build a specification
/// and hand it to the repository, keeping repositories thin and query logic
/// out of controllers. (Specification pattern, as used in eShopOnWeb.)
/// </summary>
public interface ISpecification<T>
{
    /// <summary>The WHERE predicate. Null means "no filter".</summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>Strongly-typed eager-load paths.</summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>String-based include paths for nested (ThenInclude) loading.</summary>
    List<string> IncludeStrings { get; }

    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>Secondary ordering applied after the primary sort.</summary>
    Expression<Func<T, object>>? ThenBy { get; }
    Expression<Func<T, object>>? ThenByDescending { get; }

    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }

    /// <summary>When true the evaluator emits AsNoTracking for read-only queries.</summary>
    bool AsNoTracking { get; }

    /// <summary>When true the evaluator ignores the soft-delete global query filter.</summary>
    bool IgnoreQueryFilters { get; }
}
