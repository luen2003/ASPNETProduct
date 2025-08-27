using IdentityServer.Enums;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace IdentityServer.Contexts;
public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SqlConnection");
    }

    public IDbConnection CreateConnection(RBDMSEnum rbdms = RBDMSEnum.SQLSERVER)
        => rbdms == RBDMSEnum.ORACLE ? new OracleConnection(_connectionString) : new SqlConnection(_connectionString);
}
