namespace Microsoft.AspNetCore.Builder;

public static class GetInvoiceController
{
    public static WebApplication UseGetInvoiceController(
        this WebApplication app)
    {
        app.MapGet(Endpoints.GetInvoice, Get)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> Get(
        int orderId, IGetInvoiceInputPort inputPort)
    {
        var Invoice = await inputPort.Handle(orderId);

        return Invoice is null
            ? Results.NotFound()
            : Results.Ok(Invoice);
    }
}
