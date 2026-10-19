using System.Net;

namespace NorthWind.Sales.Frontend.WebApiGateways;

internal class InvoiceGateway(HttpClient client) : IInvoiceGateway
{
    public async Task<InvoiceDto?> GetInvoiceAsync(int orderId)
    {
        try
        {
            return await client.GetFromJsonAsync<InvoiceDto>(
                $"{Endpoints.GetInvoice}?orderId={orderId}");
        }
        catch (HttpRequestException ex) when (
            ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoicesAsync(
        string? searchTerm, int page, int pageSize)
    {
        var Response =
            await client.GetFromJsonAsync<PagedResultDto<InvoiceSummaryDto>>(
                $"{Endpoints.SearchInvoices}?search={Uri.EscapeDataString(searchTerm ?? string.Empty)}" +
                $"&page={page}&pageSize={pageSize}");

        return Response!;
    }

    public async Task<byte[]> GetInvoicePdfAsync(int orderId) =>
        await client.GetByteArrayAsync(
            $"{Endpoints.GetInvoicePdf}?orderId={orderId}");
}
