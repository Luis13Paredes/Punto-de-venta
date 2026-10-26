namespace NorthWind.Sales.Frontend.WebApiGateways;

internal class SearchProductsGateway(HttpClient client)
    : ISearchProductsGateway
{
    public async Task<PagedResultDto<ProductResultDto>> SearchProductsAsync(
        string? searchTerm, int page, int pageSize, string? filterBy)
    {
        var Response = await client.GetFromJsonAsync<PagedResultDto<ProductResultDto>>(
            BuildSearchUrl(searchTerm, page, pageSize, filterBy));

        return Response!;
    }

    internal static string BuildSearchUrl(
        string? searchTerm, int page, int pageSize, string? filterBy) =>
        $"{Endpoints.SearchProducts}?search={Uri.EscapeDataString(searchTerm ?? string.Empty)}" +
        $"&page={page}&pageSize={pageSize}" +
        $"&filterBy={Uri.EscapeDataString(filterBy ?? "all")}";
}
