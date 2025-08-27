namespace IdentityServer.Helpers;
public class SQLMethodHelper
{
    #region Egas portal
    public static async Task<long> GenarateId(IDbConnection conn)
    {
        string query = @"SELECT BOS.BASETAB_SEQ.nextval As I From Dual";
        return await conn.QuerySingleOrDefaultAsync<long>(query);
    }

    public static async Task<string> GenerateCodeWithPrefix(IDbConnection conn, string prefix, string table)
    {
        string query = @"Select C From s900.{0}
                        Where C Like :Code
						Order By Cast(SUBSTR(C, INSTR(C, '.') + 1) As Number) Desc";
        var lastCode = await conn.QueryFirstOrDefaultAsync<string>(string.Format(query, table), new { code = prefix + "%" });
        if (!string.IsNullOrEmpty(prefix))
        {
            prefix += ".";
        }
        return lastCode == null ? string.Empty : string.Concat(prefix, lastCode.Split(".").Last().ToLong() + 1);
    }

    public static async Task<long> GetCustQuotaByDriverQuota(IDbConnection conn, long driverQuotaId, long transDate = 0)
    {
        string query = @"Select 
                            CustQuota.I 
	                    From S900.DriverQuota
	                    Inner Join S900.CustQuota On CustQuota.ToCustId = DriverQuota.CustId
	                    Where 
                            DriverQuota.I = :driverQuotaId 
                            And CustQuota.TQuota = 1
                            And CustQuota.ValidFromDate <= :transDate
                            And CustQuota.ValidToDate >= :transDate
	                    Order By 
		                    Case
                                When CustQuota.CustId = CustQuota.ToCustId Then 0
                                Else 1
                            End, CustQuota.ValidFromDate";
        if (transDate == 0) transDate = localsetting.SQLgetDate();
        return await conn.QueryFirstOrDefaultAsync<long>(query, new { driverQuotaId, transDate });
    }

    public static async Task<bool> SyncHist(IDbConnection conn, string tableName, long id, long userId, IDbTransaction transaction = null!)
    {
        string queryGetCols = @"SELECT column_name
                                FROM USER_TAB_COLUMNS
                                WHERE table_name = UPPER('{0}')
                                Order By column_id";
        queryGetCols = string.Format(queryGetCols, tableName);
        var listCols = await conn.QueryAsync<string>(queryGetCols, transaction: transaction);
        string hist = string.Concat(tableName, "_hist");
        var query = @"Insert Into {0} ({1}) Select * From {2} Where I = :Id";
        var queryUpdate = @"Update {0} Set ChangeU = :UserId, ChangeD = :DateTimeNow Where I = :Id";
        query = string.Format(query, hist, string.Join(",", listCols), tableName);
        queryUpdate = string.Format(queryUpdate, hist);
        await conn.ExecuteAsync(query, new { id }, transaction);
        var updated = await conn.ExecuteAsync(queryUpdate, new { userId, id, DateTimeNow = localsetting.SQLgetDate() }, transaction);
        return updated > 0;
    }
    #endregion
}
