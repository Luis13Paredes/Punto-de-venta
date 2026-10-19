namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Invoice;

public interface IGetInvoiceInputPort
{
    Task<InvoiceDto?> Handle(int orderId);
}
