namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Queries;

public interface ISearchProductsInputPort
{
    Task<PagedResultDto<ProductResultDto>> Handle(
        string? searchTerm, int page, int pageSize);
}
