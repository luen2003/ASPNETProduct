using BOSDLL.BOSL;
using Dapper;
using ServiceAPI.Context;
using ServiceAPI.Models.MD;
using ServiceAPI.Repositories.Interfaces;
using ServiceAPI.Utils;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _schemal = "S168";
        private readonly DapperContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public CustomerRepository(DapperContext context, ILogger<TransactionRepository> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }
        public async Task<List<Customer>> GetAllData()
        {
            var query = @"Select C.C As custcode, C.N As custName, C.ShortN As shortName , C.TaxCode As TaxCode,
                        C.Tel As tel, C.Fax As fax, C.Address As address, C.Province As province, 
                        C.SysD As sysD, C.EStatus As status
                        From MD.Customer C "; 

            using IDbConnection connection = _context.CreateConnection();
            var Customer = await connection.QueryAsync<Customer>(query); 
            return Customer.ToList();
        }

        public async Task<List<Customer>> GetDatabyCustomerCode(string customerCode)
        {
            var query = @"Select C.C As custcode, C.N As custName, C.ShortN As shortName , C.TaxCode As TaxCode,
                        C.Tel As tel, C.Fax As fax, C.Address As address, C.Province As province, 
                        C.SysD As sysD, C.EStatus As status
                        From MD.Customer C where C = @customerCode ";
            var param = new DynamicParameters();
            if (customerCode != "")
            {
                param.Add(name: "customerCode", customerCode);
            }
            using IDbConnection connection = _context.CreateConnection();
            var Customer = await connection.QueryAsync<Customer>(query, param);
            return Customer.ToList();
        }

    }
}
