namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;

internal class NorthWindSalesContextFactory :
    IDesignTimeDbContextFactory<NorthWindSalesContext>
{
    public NorthWindSalesContext CreateDbContext(string[] args)
    {
        // Design-time connection must match the runtime DBOptions
        // connection string (appsettings.json of NorthWind.Sales.WebApi),
        // otherwise migrations are applied to a different database.
        IOptions<DBOptions> DbOptions =
            Microsoft.Extensions.Options.Options.Create(
                new DBOptions
                {
                    ConnectionString =
                        @"Server=localhost\SQLEXPRESS;Database=NorthWindDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True;",
                    DomainLogsConnectionString = string.Empty
                });

        return new NorthWindSalesContext(DbOptions);
    }
}
