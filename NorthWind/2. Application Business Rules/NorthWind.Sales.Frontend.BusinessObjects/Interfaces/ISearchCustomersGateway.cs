namespace NorthWind.Sales.Frontend.BusinessObjects.Interfaces;

public interface ISearchCustomersGateway
{
    Task<PagedResultDto<CustomerResultDto>> SearchCustomersAsync(
        string? searchTerm, int page, int pageSize,
        string? filterBy);
}
