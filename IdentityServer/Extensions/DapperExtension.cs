namespace IdentityServer.Extensions;
public class DapperExtension
{
    public static async Task<bool> ExecuteTransactionAsync(IDbConnection conn, Func<IDbTransaction, Task> execution)
    {
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            await execution(transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
