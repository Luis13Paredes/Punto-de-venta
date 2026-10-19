namespace NorthWind.Sales.Frontend.BusinessObjects.Interfaces;

public interface ISearchProductsGateway
{
    Task<PagedResultDto<ProductResultDto>> SearchProductsAsync(
        string? searchTerm, int page, int pageSize);
}
