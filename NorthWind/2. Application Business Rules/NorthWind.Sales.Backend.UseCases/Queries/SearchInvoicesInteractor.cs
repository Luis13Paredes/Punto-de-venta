namespace NorthWind.Sales.Backend.UseCases.Queries;

internal class SearchInvoicesInteractor(
    IQueriesRepository repository,
    IUserService userService) : ISearchInvoicesInputPort
{
    public Task<PagedResultDto<InvoiceSummaryDto>> Handle(
        string? searchTerm, int page, int pageSize)
    {
        GuardUser.AgainstUnauthenticated(userService);

        return repository.SearchInvoices(searchTerm, page, pageSize);
    }
}
