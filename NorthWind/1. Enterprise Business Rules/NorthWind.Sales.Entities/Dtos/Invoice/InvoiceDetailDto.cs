namespace NorthWind.Sales.Entities.Dtos.Invoice;

public class InvoiceDetailDto(
    int productId, string productName, decimal unitPrice, short quantity)
{
    public int ProductId => productId;
    public string ProductName => productName;
    public decimal UnitPrice => unitPrice;
    public short Quantity => quantity;
    public decimal LineTotal => Math.Round(
        unitPrice * quantity, 2, MidpointRounding.AwayFromZero);
}
