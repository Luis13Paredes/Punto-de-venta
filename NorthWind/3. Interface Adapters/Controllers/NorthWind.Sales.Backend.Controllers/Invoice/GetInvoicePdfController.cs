namespace Microsoft.AspNetCore.Builder;

public static class GetInvoicePdfController
{
    public static WebApplication UseGetInvoicePdfController(
        this WebApplication app)
    {
        app.MapGet(Endpoints.GetInvoicePdf, GetPdf)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetPdf(
        int orderId, IGetInvoiceInputPort inputPort)
    {
        var Invoice = await inputPort.Handle(orderId);

        if (Invoice is null)
            return Results.NotFound();

        var Pdf = InvoicePdfGenerator.Generate(Invoice);

        return Results.File(Pdf, "application/pdf",
            $"Invoice-{orderId}.pdf");
    }
}
