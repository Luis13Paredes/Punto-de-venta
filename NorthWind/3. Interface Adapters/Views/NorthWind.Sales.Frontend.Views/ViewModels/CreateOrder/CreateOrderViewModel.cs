namespace NorthWind.Sales.Frontend.Views.ViewModels.CreateOrder;

public class CreateOrderViewModel(ICreateOrderGateway gateway,
    ISearchCustomersGateway searchCustomersGateway,
    ISearchProductsGateway searchProductsGateway,
    IModelValidatorHub<CreateOrderViewModel> validator)
{
    public IModelValidatorHub<CreateOrderViewModel> Validator => validator;

    public ModelValidator<CreateOrderViewModel>? ModelValidatorComponentReference { get; set; }

    #region Propiedades relacionadas a CreateOrderDto
    public string CustomerId { get; set; } = string.Empty;
    public string ShipAddress { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public List<CreateOrderDetailViewModel> OrderDetails { get; set; } = [];
    #endregion

    // Display-only: name of the customer picked in the search modal.
    public string CustomerName { get; set; } = string.Empty;

    public string InformationMessage { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;

    // Requirement 4: totals shown in the point of sale form.
    // Keep in sync with OrderTotals.TaxRateValue (backend source of truth).
    public const decimal TaxRate = 0.15m;
    public decimal Subtotal =>
        OrderDetails.Sum(d => d.UnitPrice * d.Quantity);
    public decimal TaxAmount =>
        Math.Round(Subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
    public decimal Total => Subtotal + TaxAmount;

    public void AddNewOrderDetailItem()
    {
        ErrorMessage = string.Empty;
        OrderDetails.Add(
            new CreateOrderDetailViewModel());
    }

    #region Smart search modals (Requirement 15)
    public Task<PagedResultDto<CustomerResultDto>> SearchCustomers(
        string? searchTerm, int page, int pageSize) =>
        searchCustomersGateway.SearchCustomersAsync(
            searchTerm, page, pageSize);

    public Task<PagedResultDto<ProductResultDto>> SearchProducts(
        string? searchTerm, int page, int pageSize) =>
        searchProductsGateway.SearchProductsAsync(
            searchTerm, page, pageSize);

    // Requirement 5: only the needed data is captured from the modal.
    public void SelectCustomer(CustomerResultDto customer)
    {
        ErrorMessage = string.Empty;
        CustomerId = customer.Id;
        CustomerName = customer.Name;
    }
    #endregion

    #region Product actions (Requirements 8, 10 and 11)
    // Requirement 8: products come from the modal filtered by stock > 0.
    public void AddProductFromSearch(ProductResultDto product)
    {
        ErrorMessage = string.Empty;
        if (OrderDetails.Any(d => d.ProductId == product.Id))
        {
            ErrorMessage =
                CreateOrderMessages.ProductAlreadyInOrderMessage;
            return;
        }

        OrderDetails.Add(new CreateOrderDetailViewModel
        {
            ProductId = product.Id,
            UnitPrice = product.UnitPrice,
            Quantity = 1
        });
    }

    // Requirement 11: change a product keeping the rest of the data.
    public void ReplaceOrderDetailProduct(
        CreateOrderDetailViewModel detail, ProductResultDto product)
    {
        ErrorMessage = string.Empty;
        if (OrderDetails.Any(d => d != detail && d.ProductId == product.Id))
        {
            ErrorMessage =
                CreateOrderMessages.ProductAlreadyInOrderMessage;
            return;
        }

        detail.ProductId = product.Id;
        detail.UnitPrice = product.UnitPrice;
    }

    // Requirement 10: remove a product before saving the order.
    public void RemoveOrderDetail(CreateOrderDetailViewModel detail)
    {
        ErrorMessage = string.Empty;
        OrderDetails.Remove(detail);
    }
    #endregion

    public async Task Send()
    {
        InformationMessage = "";
        ErrorMessage = "";

        // Requirement: an order cannot contain the same product twice.
        var DuplicatedIds = OrderDetails
            .Where(d => d.ProductId > 0)
            .GroupBy(d => d.ProductId)
            .Where(Group => Group.Count() > 1)
            .Select(Group => Group.Key)
            .OrderBy(Id => Id)
            .ToList();

        if (DuplicatedIds.Count > 0)
        {
            ErrorMessage = string.Format(
                CreateOrderMessages.DuplicateProductsMessage,
                string.Join(", ", DuplicatedIds));
            return;
        }

        try
        {
            var OrderId = await gateway.CreateOrderAsync(
                (CreateOrderDto)this);
            InformationMessage = string.Format(
                CreateOrderMessages.TransactionCompletedMessage, OrderId);
        }
        catch (HttpRequestException ex)
        {
            if (ex.Data.Contains("Errors"))
            {
                var Errors =
                    ex.Data["Errors"] as IEnumerable<ValidationError>;
                ModelValidatorComponentReference?.AddErrors(Errors!);
            }
            else
            {
                // Requirement 17: show that the transaction failed.
                ErrorMessage =
                    CreateOrderMessages.TransactionFailedMessage;
            }
        }
    }

    public static explicit operator CreateOrderDto(
        CreateOrderViewModel model) =>
        new CreateOrderDto(
            model.CustomerId, model.ShipAddress, model.ShipCity,
            model.ShipCountry, model.ShipPostalCode,
            model.OrderDetails.Select(d => new CreateOrderDetailDto(
                d.ProductId, d.UnitPrice, d.Quantity)
            ));
}
