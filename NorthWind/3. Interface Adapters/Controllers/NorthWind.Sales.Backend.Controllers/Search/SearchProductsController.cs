namespace Microsoft.AspNetCore.Builder;

public static class SearchProductsController
{
    public static WebApplication UseSearchProductsController(
        this WebApplication app)
    {
        app.MapGet(Endpoints.SearchProducts, Search)
            .RequireAuthorization();

        return app;
    }

    public static async Task<PagedResultDto<ProductResultDto>> Search(
        string? search, int? page, int? pageSize, string? filterBy,
        ISearchProductsInputPort inputPort) =>
        await inputPort.Handle(
            search, page ?? 1, pageSize ?? 5, filterBy);
}
