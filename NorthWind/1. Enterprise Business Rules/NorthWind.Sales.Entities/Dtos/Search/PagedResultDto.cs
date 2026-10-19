namespace NorthWind.Sales.Entities.Dtos.Search;

public class PagedResultDto<T>(
    IReadOnlyList<T> items, int page, int pageSize, int totalCount)
{
    public IReadOnlyList<T> Items => items;
    public int Page => page;
    public int PageSize => pageSize;
    public int TotalCount => totalCount;
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
