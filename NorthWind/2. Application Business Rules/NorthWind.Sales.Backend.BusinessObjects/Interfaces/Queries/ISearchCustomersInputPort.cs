namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Queries;

public interface ISearchCustomersInputPort
{
    Task<PagedResultDto<CustomerResultDto>> Handle(
        string? searchTerm, int page, int pageSize);
}
