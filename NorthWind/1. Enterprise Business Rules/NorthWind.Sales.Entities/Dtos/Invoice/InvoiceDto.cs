namespace NorthWind.Sales.Entities.Dtos.Invoice;

public class InvoiceDto(
    int orderId,
    DateTime orderDate,
    string customerId,
    string customerName,
    string shipAddress,
    string shipCity,
    string shipCountry,
    string shipPostalCode,
    decimal subtotal,
    decimal tax,
    decimal total,
    IReadOnlyList<InvoiceDetailDto> details)
{
    public int OrderId => orderId;
    public DateTime OrderDate => orderDate;
    public string CustomerId => customerId;
    public string CustomerName => customerName;
    public string ShipAddress => shipAddress;
    public string ShipCity => shipCity;
    public string ShipCountry => shipCountry;
    public string ShipPostalCode => shipPostalCode;
    public decimal Subtotal => subtotal;
    public decimal Tax => tax;
    public decimal Total => total;
    public IReadOnlyList<InvoiceDetailDto> Details => details;
}
