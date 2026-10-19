namespace NorthWind.Sales.Entities.Dtos.Invoice;

public class InvoiceSummaryDto(
    int orderId, DateTime orderDate,
    string customerId, string customerName, decimal total)
{
    public int OrderId => orderId;
    public DateTime OrderDate => orderDate;
    public string CustomerId => customerId;
    public string CustomerName => customerName;
    public decimal Total => total;
}
