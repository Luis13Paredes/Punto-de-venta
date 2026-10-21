namespace NorthWind.Sales.Validators.Entities.CreateOrder;

internal class CreateOrderDtoValidator :
    AbstractModelValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator(
        IValidationService<CreateOrderDto> validationService,
        IModelValidator<CreateOrderDetailDto> detailValidator) :
        base(validationService)
    {
        AddRuleFor(c => c.CustomerId)
            .NotEmpty(CreateOrderMessages.CustomerIdRequired)
            .Must(CustomerIdHasValidFormat,
                CreateOrderMessages.CustomerIdInvalidFormat);

        AddRuleFor(c => c.ShipAddress)
            .NotEmpty(CreateOrderMessages.ShipAddressRequired)
            .MaximumLength(60, CreateOrderMessages.ShipAddressMaximumLength);

        AddRuleFor(c => c.ShipCity)
            .NotEmpty(CreateOrderMessages.ShipCityRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCityMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCityMaximumLength)
            .Matches(@"^[A-Za-zÁÉÍÓÚÜáéíóúüñÑ ]+$",
                CreateOrderMessages.ShipCityOnlyLetters);

        AddRuleFor(c => c.ShipCountry)
            .NotEmpty(CreateOrderMessages.ShipCountryRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCountryMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCountryMaximumLength)
            .Matches(@"^[A-Za-zÁÉÍÓÚÜáéíóúüñÑ ]+$",
                CreateOrderMessages.ShipCountryOnlyLetters);

        AddRuleFor(c => c.ShipPostalCode)
            .NotEmpty(CreateOrderMessages.ShipPostalCodeRequired)
            .MaximumLength(10, CreateOrderMessages.ShipPostalCodeMaximumLength)
            .Matches(@"^[0-9]*$",
                CreateOrderMessages.ShipPostalCodeOnlyDigits);

        AddRuleFor(c => c.OrderDetails)
            .NotNull(CreateOrderMessages.OrderDetailsRequired)
            .NotEmpty(CreateOrderMessages.OrderDetailsNotEmpty);

        AddRuleForEach(c => c.OrderDetails)
            .SetValidator(detailValidator);
    }

    // Requirement 6: the customer id must be the 5 character legacy code.
    static bool CustomerIdHasValidFormat(string? customerId) =>
        customerId is not null && customerId.Length == 5;
}
