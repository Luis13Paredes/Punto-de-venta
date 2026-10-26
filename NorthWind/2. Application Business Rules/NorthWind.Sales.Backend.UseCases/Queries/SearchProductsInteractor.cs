namespace NorthWind.Sales.Backend.UseCases.Queries;

internal class SearchProductsInteractor(
    IQueriesRepository repository,
    IUserService userService) : ISearchProductsInputPort
{
    public Task<PagedResultDto<ProductResultDto>> Handle(
        string? searchTerm, int page, int pageSize, string? filterBy)
    {
        GuardUser.AgainstUnauthenticated(userService);

        return repository.SearchProducts(
            searchTerm, page, pageSize, filterBy);
    }
}
