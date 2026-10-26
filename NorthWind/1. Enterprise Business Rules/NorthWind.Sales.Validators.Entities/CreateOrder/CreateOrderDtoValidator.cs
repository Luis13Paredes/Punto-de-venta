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
            .StopOnFirstFailure()
            .NotEmpty(CreateOrderMessages.ShipAddressRequired)
            .MaximumLength(30, CreateOrderMessages.ShipAddressMaximumLength)
            .Matches(@"^[A-Za-zÁÉÍÓÚÜáéíóúüñÑ0-9 .,#°-]+$",
                CreateOrderMessages.ShipAddressInvalidFormat);

        AddRuleFor(c => c.ShipCity)
            .StopOnFirstFailure()
            .NotEmpty(CreateOrderMessages.ShipCityRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCityMinimumLength)
            .MaximumLength(30, CreateOrderMessages.ShipCityMaximumLength)
            .Matches(@"^[A-Za-zÁÉÍÓÚÜáéíóúüñÑ ]+$",
                CreateOrderMessages.ShipCityOnlyLetters);

        AddRuleFor(c => c.ShipCountry)
            .StopOnFirstFailure()
            .NotEmpty(CreateOrderMessages.ShipCountryRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCountryMinimumLength)
            .MaximumLength(30, CreateOrderMessages.ShipCountryMaximumLength)
            .Matches(@"^[A-Za-zÁÉÍÓÚÜáéíóúüñÑ ]+$",
                CreateOrderMessages.ShipCountryOnlyLetters);

        AddRuleFor(c => c.ShipPostalCode)
            .StopOnFirstFailure()
            .NotEmpty(CreateOrderMessages.ShipPostalCodeRequired)
            .MaximumLength(6, CreateOrderMessages.ShipPostalCodeMaximumLength)
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
