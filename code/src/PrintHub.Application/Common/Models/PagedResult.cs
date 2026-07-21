namespace PrintHub.Application.Common.Models;

/// <summary>
/// Standardised paged envelope returned by every list endpoint, so pagination
/// behaves identically across the whole API (BR-22).
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

/// <summary>Normalised paging request; clamps page/size into safe bounds.</summary>
public class PageRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? Math.Clamp(value, 1, MaxPageSize) : value;
    }

    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
