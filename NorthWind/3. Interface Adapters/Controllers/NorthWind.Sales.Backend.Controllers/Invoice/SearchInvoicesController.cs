namespace Microsoft.AspNetCore.Builder;

public static class SearchInvoicesController
{
    public static WebApplication UseSearchInvoicesController(
        this WebApplication app)
    {
        app.MapGet(Endpoints.SearchInvoices, Search)
            .RequireAuthorization();

        return app;
    }

    public static async Task<PagedResultDto<InvoiceSummaryDto>> Search(
        string? search, int? page, int? pageSize,
        ISearchInvoicesInputPort inputPort) =>
        await inputPort.Handle(
            search, page ?? 1, pageSize ?? 5);
}
