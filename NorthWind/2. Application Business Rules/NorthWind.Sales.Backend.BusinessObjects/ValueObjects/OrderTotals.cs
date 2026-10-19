namespace NorthWind.Sales.Backend.BusinessObjects.ValueObjects;

/// <summary>
/// Totals of an order: subtotal, tax and grand total.
/// </summary>
public record OrderTotals(
    decimal Subtotal,
    decimal TaxRate,
    decimal TaxAmount,
    decimal Total)
{
    // Ecuador tax rate (IVA): 15% since April 1st, 2024
    // (Executive Decree No. 198).
    // Update this constant if the rate changes and rebuild.
    public const decimal TaxRateValue = 0.15m;

    public static OrderTotals Calculate(
        IEnumerable<OrderDetail> orderDetails)
    {
        decimal Subtotal = orderDetails
            .Sum(detail => detail.UnitPrice * detail.Quantity);

        decimal TaxAmount = Math.Round(
            Subtotal * TaxRateValue, 2, MidpointRounding.AwayFromZero);

        return new OrderTotals(
            Subtotal, TaxRateValue, TaxAmount, Subtotal + TaxAmount);
    }
}
