namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICreateOrderInputPort, CreateOrderInteractor>();
        services.AddScoped<ISearchCustomersInputPort, SearchCustomersInteractor>();
        services.AddScoped<ISearchProductsInputPort, SearchProductsInteractor>();
        services.AddScoped<IGetInvoiceInputPort, GetInvoiceInteractor>();
        services.AddScoped<ISearchInvoicesInputPort, SearchInvoicesInteractor>();

        services.AddModelValidator<CreateOrderDto,
            CreateOrderCustomerValidator>();
        services.AddModelValidator<CreateOrderDto,
            CreateOrderProductValidator>();

        services.AddScoped<IDomainEventHandler<SpecialOrderCreatedEvent>,
            SendEMailWhenSpecialOrderCreatedEventHandler>();

        return services;
    }
}
