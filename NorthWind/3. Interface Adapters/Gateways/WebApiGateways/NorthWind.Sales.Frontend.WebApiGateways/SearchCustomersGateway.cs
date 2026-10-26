namespace NorthWind.Sales.Frontend.WebApiGateways;

internal class SearchCustomersGateway(HttpClient client)
    : ISearchCustomersGateway
{
    public async Task<PagedResultDto<CustomerResultDto>> SearchCustomersAsync(
        string? searchTerm, int page, int pageSize, string? filterBy)
    {
        var Response = await client.GetFromJsonAsync<PagedResultDto<CustomerResultDto>>(
            BuildSearchUrl(searchTerm, page, pageSize, filterBy));

        return Response!;
    }

    internal static string BuildSearchUrl(
        string? searchTerm, int page, int pageSize, string? filterBy) =>
        $"{Endpoints.SearchCustomers}?search={Uri.EscapeDataString(searchTerm ?? string.Empty)}" +
        $"&page={page}&pageSize={pageSize}" +
        $"&filterBy={Uri.EscapeDataString(filterBy ?? "all")}";
}
