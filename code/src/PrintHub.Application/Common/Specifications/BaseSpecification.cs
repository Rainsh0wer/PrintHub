using System.Linq.Expressions;

namespace PrintHub.Application.Common.Specifications;

/// <summary>
/// Base class for concrete specifications. Derived specs use the protected
/// builder methods in their constructor to declare criteria, includes, ordering,
/// and paging. Concrete specs live in the Application layer with no dependency
/// on EF Core; the Infrastructure layer's evaluator translates them to queries.
/// </summary>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification() { }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();

    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public Expression<Func<T, object>>? ThenBy { get; private set; }
    public Expression<Func<T, object>>? ThenByDescending { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public bool AsNoTracking { get; private set; }
    public bool IgnoreQueryFilters { get; private set; }

    protected void Where(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByExpression) => OrderByDescending = orderByExpression;
    protected void ApplyThenBy(Expression<Func<T, object>> expr) => ThenBy = expr;
    protected void ApplyThenByDescending(Expression<Func<T, object>> expr) => ThenByDescending = expr;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void AsReadOnly() => AsNoTracking = true;
    protected void IgnoreFilters() => IgnoreQueryFilters = true;
}
