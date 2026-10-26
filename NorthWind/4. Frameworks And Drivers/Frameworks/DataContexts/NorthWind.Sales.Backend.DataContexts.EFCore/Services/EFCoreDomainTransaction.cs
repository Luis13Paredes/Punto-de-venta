namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

// Portable transaction (Windows and Linux): uses the DbContext's own
// EF Core transaction instead of System.Transactions/TransactionScope,
// which requires MSDTC for multi-database scopes and throws
// PlatformNotSupportedException on Linux.
internal class EFCoreDomainTransaction(
    INorthWindSalesCommandsDataContext dataContext) :
    IDomainTransaction, IDisposable
{
    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? Transaction;

    public void BeginTransaction()
    {
        if (dataContext is DbContext dbContext)
            Transaction = dbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        Transaction?.Commit();
        Dispose();
    }

    public void RollbackTransaction() => Dispose();

    public void Dispose() => Transaction?.Dispose();
}
