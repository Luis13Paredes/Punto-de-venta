namespace NorthWind.Sales.Backend.BusinessObjects.Aggregates;

public class OrderAggregate : Order
{
    readonly List<OrderDetail> OrderDetailsField = [];
    public IReadOnlyCollection<OrderDetail> OrderDetails => OrderDetailsField;

    public void AddDetail(int productId, decimal unitPrice, short quantity)
    {
        var ExistingOrderDetail =
            OrderDetailsField
            .FirstOrDefault(o => o.ProductId == productId);

        if (ExistingOrderDetail != default)
        {
            quantity += ExistingOrderDetail.Quantity;
            OrderDetailsField.Remove(ExistingOrderDetail);
        }

        OrderDetailsField.Add(
            new OrderDetail(productId, unitPrice, quantity));
    }

    public static OrderAggregate From(CreateOrderDto orderDto)
    {
        OrderAggregate OrderAggregate = new OrderAggregate
        {
            CustomerId = orderDto.CustomerId,
            ShipAddress = orderDto.ShipAddress,
            ShipCity = orderDto.ShipCity,
            ShipCountry = orderDto.ShipCountry,
            ShipPostalCode = orderDto.ShipPostalCode
        };

        foreach (var Item in orderDto.OrderDetails)
        {
            OrderAggregate.AddDetail(Item.ProductId,
                Item.UnitPrice, Item.Quantity);
        }

        // Requirement 4: calculate subtotal, tax (IVA) and total.
        OrderTotals Totals = OrderTotals.Calculate(
            OrderAggregate.OrderDetails);
        OrderAggregate.Subtotal = Totals.Subtotal;
        OrderAggregate.Tax = Totals.TaxAmount;
        OrderAggregate.Total = Totals.Total;

        return OrderAggregate;
    }
}
