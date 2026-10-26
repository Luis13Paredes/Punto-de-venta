namespace Microsoft.AspNetCore.Builder;

public static class SearchCustomersController
{
    public static WebApplication UseSearchCustomersController(
        this WebApplication app)
    {
        app.MapGet(Endpoints.SearchCustomers, Search)
            .RequireAuthorization();

        return app;
    }

    public static async Task<PagedResultDto<CustomerResultDto>> Search(
        string? search, int? page, int? pageSize, string? filterBy,
        ISearchCustomersInputPort inputPort) =>
        await inputPort.Handle(
            search, page ?? 1, pageSize ?? 5, filterBy);
}
