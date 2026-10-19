namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddWebApiGateways(
        this IServiceCollection services,
        Action<HttpClient> configureHttpClient,
        Action<IHttpClientBuilder> configureHttpClientBuilder)
    {
        services.AddExceptionDelegatingHandler();

        // Every gateway requires the bearer token handler: all of the
        // sales endpoints are protected with RequireAuthorization.
        var CreateOrderBuilder = services
            .AddHttpClient<ICreateOrderGateway,
                CreateOrderGateway>(configureHttpClient)
            .AddHttpMessageHandler<ExceptionDelegatingHandler>();
        configureHttpClientBuilder(CreateOrderBuilder);

        var SearchCustomersBuilder = services
            .AddHttpClient<ISearchCustomersGateway,
                SearchCustomersGateway>(configureHttpClient)
            .AddHttpMessageHandler<ExceptionDelegatingHandler>();
        configureHttpClientBuilder(SearchCustomersBuilder);

        var SearchProductsBuilder = services
            .AddHttpClient<ISearchProductsGateway,
                SearchProductsGateway>(configureHttpClient)
            .AddHttpMessageHandler<ExceptionDelegatingHandler>();
        configureHttpClientBuilder(SearchProductsBuilder);

        var InvoiceBuilder = services
            .AddHttpClient<IInvoiceGateway,
                InvoiceGateway>(configureHttpClient)
            .AddHttpMessageHandler<ExceptionDelegatingHandler>();
        configureHttpClientBuilder(InvoiceBuilder);

        return services;
    }
}
