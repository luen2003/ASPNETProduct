using Azure.Core;
using BOSDLL.BOSL;
using Dapper;
using ServiceAPI.Context;
using ServiceAPI.Models.MD;
using ServiceAPI.Models.TransactionResponse;
using ServiceAPI.Repositories.Interfaces;
using ServiceAPI.Utils;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceAPI.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly string _schemal = "S168";
        private readonly DapperContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public VehicleRepository(DapperContext context, ILogger<TransactionRepository> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }
        public async Task<List<Vehicle>> GetAllData()
        {
            var query = @"Select C.C As vehicleCode, C.N As vehicleName From MD.Utrans C "; 

            using IDbConnection connection = _context.CreateConnection();
            var Customer = await connection.QueryAsync<Vehicle>(query); 
            return Customer.ToList();
        }

        public async Task<List<Vehicle>> GetDatabyVehicleCode(string vehicleCode)
        {
            var query = @"Select C.C As vehicleCode, C.N As vehicleName From MD.Utrans C where C = @vehicleCode ";
            var param = new DynamicParameters();
            if (vehicleCode != "")
            {
                param.Add(name: "vehicleCode", vehicleCode);
            }
            using IDbConnection connection = _context.CreateConnection();
            var Customer = await connection.QueryAsync<Vehicle>(query, param);
            return Customer.ToList();
        }

        public async Task<ResponseSaveVehicle> SaveVehicleNewRepo(VehicleSaveRequest request)
        {
            bool CheckErr = true;
            using IDbConnection connection = _context.CreateConnection();
            ResponseSaveVehicle responseSaveVehicle = new();
            try
            {
                for (int i = 0; i < request.data.Count(); i++)
                {
                    var data = await CreateVehicle(request.data[i], connection);
                    if (data.errorCode == "00")
                    {
                        responseSaveVehicle.listSuccess += data.listSuccess + ";";
                    }
                    else
                    {
                        CheckErr = false;
                        responseSaveVehicle.listFail += data.listFail + ";";
                        responseSaveVehicle.description += data.description + ";";
                    }
                }
                if (CheckErr)
                    responseSaveVehicle.errorCode = "00";
                else
                    responseSaveVehicle.errorCode = "01";
            }
            catch (Exception ex)
            {

                if (ex.StackTrace.Contains("System.DateTime.ParseExact"))
                {
                    responseSaveVehicle.errorCode = "04";
                    responseSaveVehicle.description += " Vui lòng kiểm tra lại thông tin ngày tháng năm ";
                }
                else if (ex.StackTrace.Contains("Microsoft.Data.SqlClient.SqlConnection"))
                {
                    responseSaveVehicle.errorCode = "66";
                    responseSaveVehicle.description += " Kiểm tra lại câu lệnh sql ";
                }
                else
                {
                    responseSaveVehicle.errorCode = "04";
                    responseSaveVehicle.description += ex.StackTrace + "_" + ex.ToString() + " ";
                }

                return responseSaveVehicle;
            }

            return responseSaveVehicle;
        }

        public async Task<ResponseSaveVehicle> CreateVehicle(VehicleRequest cmd, IDbConnection connection = null)
        {
            ResponseSaveVehicle vehicleResponse = new();
            var conn = connection == null ? _context.CreateConnection() : connection;
            var errStr = "";
            var parameters = new DynamicParameters();
            int rowAffected = 0;
            string strStockType = "";
            try
            {
                if (string.IsNullOrEmpty(cmd.vehicleCode))
                {
                    vehicleResponse.errorCode = "02";
                    vehicleResponse.description = "Mã lệnh không được để trống ";
                    vehicleResponse.listFail = cmd.vehicleCode;
                    return vehicleResponse;
                }

                cmd.vehicleCode = cmd.vehicleCode.Replace(" ", "").Replace("-", "").Replace(".", "");

                var IDVehicle = await GetIdbyCode(conn, "MD.Utrans", cmd.vehicleCode);
                if (IDVehicle != 0)
                {
                    vehicleResponse.errorCode = "20";
                    vehicleResponse.description = "Mã phương tiện "+ cmd.vehicleCode + " đã tồn tại ";
                    vehicleResponse.listFail = cmd.vehicleCode;
                    return vehicleResponse;
                }
                 
                var IDVehicleGroup = await GetIdMapbyCode(conn, "MD.VehicleGroup", cmd.vehicleGroup);
                if (IDVehicleGroup != 0)
                    parameters.Add("VehicleGroup", IDVehicleGroup);
                else
                {
                    vehicleResponse.errorCode = "21";
                    vehicleResponse.description = "Mã nhóm phương tiện " + cmd.vehicleGroup + " không tồn tại ";
                    vehicleResponse.listFail += cmd.vehicleCode + "_" + cmd.vehicleGroup;
                    return vehicleResponse;
                }

                string commonSqlInsert = "I,C,N,RegNumber,VehicleStatus,OwnerType,VehicleGroup,Opt1"; 
                string dataSqlInsert = $"(@I,@C,@N,@RegNumber,1,-1,@VehicleGroup,1)";
                string sqlInsert = $"INSERT INTO MD.Utrans ({commonSqlInsert}) VALUES {dataSqlInsert};";
                long I;

                parameters.Add("C", cmd.vehicleCode);
                parameters.Add("N", cmd.vehicleCode);
                parameters.Add("RegNumber", cmd.vehicleCode);  
               
                I = await GetIDRunnerSEQ(conn);
                parameters.Add("I", I);

                // Thực hiện insert dữ liệu bảng T2011
                rowAffected = await conn.ExecuteAsync(sqlInsert, parameters); 

                if (connection == null) conn.Dispose();
                if (rowAffected != 1 || errStr != "")
                {
                    vehicleResponse.errorCode = "01";
                    vehicleResponse.description = errStr;
                    vehicleResponse.listFail = cmd.vehicleCode;
                }
                else
                {
                    vehicleResponse.errorCode = "00";
                    vehicleResponse.listSuccess = cmd.vehicleCode;
                }
            }
            catch (Exception ex)
            {
                vehicleResponse.errorCode = "04";
                vehicleResponse.description = ex.StackTrace + ex.ToString();
            }

            return vehicleResponse;
        }

        public async Task<int> GetIDRunnerSEQ(IDbConnection conn)
        {
            string sql = $"select top 1 I + 1 AS Result from MD.Utrans where 1=1 order by i desc";
            int runner = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return runner;
        }

        public async Task<int> GetIdbyCode(IDbConnection conn, string tab, string Code)
        {
            string sql = $"select top 1 I  from MD.Utrans where C = '{Code}' ";
            int ID = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return ID;
        }

        public async Task<int> GetIdMapbyCode(IDbConnection conn, string tab, string Code)
        {
            string sql = $"select top 1 I  from {tab} where C = '{Code}' ";
            int ID = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return ID;
        }
    }
}
