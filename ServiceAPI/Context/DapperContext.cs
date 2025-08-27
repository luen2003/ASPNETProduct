using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using ServiceAPI.Enums;
using System.Data;

namespace ServiceAPI.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection") ?? string.Empty;
        }

        public IDbConnection CreateConnection(RBDMSEnum rbdms = RBDMSEnum.SQLSERVER)
        => rbdms == RBDMSEnum.ORACLE ? new OracleConnection(_connectionString) : new SqlConnection(_connectionString);
    }
}
