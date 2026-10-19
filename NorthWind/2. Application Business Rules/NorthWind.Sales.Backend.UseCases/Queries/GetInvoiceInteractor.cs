namespace NorthWind.Sales.Backend.UseCases.Queries;

internal class GetInvoiceInteractor(
    IQueriesRepository repository,
    IUserService userService) : IGetInvoiceInputPort
{
    public Task<InvoiceDto?> Handle(int orderId)
    {
        GuardUser.AgainstUnauthenticated(userService);

        return repository.GetInvoice(orderId);
    }
}
