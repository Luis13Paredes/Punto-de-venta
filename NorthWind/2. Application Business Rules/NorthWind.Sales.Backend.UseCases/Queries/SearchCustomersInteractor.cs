namespace NorthWind.Sales.Backend.UseCases.Queries;

internal class SearchCustomersInteractor(
    IQueriesRepository repository,
    IUserService userService) : ISearchCustomersInputPort
{
    public Task<PagedResultDto<CustomerResultDto>> Handle(
        string? searchTerm, int page, int pageSize)
    {
        GuardUser.AgainstUnauthenticated(userService);

        return repository.SearchCustomers(searchTerm, page, pageSize);
    }
}
