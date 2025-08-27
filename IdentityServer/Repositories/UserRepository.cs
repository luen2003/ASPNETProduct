using IdentityServer.Converters;
using IdentityServer.Helpers;
using IdentityServer.Contexts;

namespace IdentityServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger _logger;
        public UserRepository(DapperContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var query = @"SELECT [I] Id
                                ,[C] UserName
                                ,[N] FullName
                                ,[ShortN] ShortName
                                ,[pwd] Password
                                ,[UType] UserType
                                ,[status] Status
                                ,[pwdexpireDay] PasswordExpireDay
                                ,[lastpwdchanged] LastPasswordChanged
                                ,[Company] CompanyId
                                ,[POS] PosId
                                ,[Tel] Tel
                                ,[Mobile] Mobile
                                ,[Addr] Address
                                ,[MailBox] Email
                                
                                ,[SysLoginInfo] SysLoginInfo
                                ,[SysU] UserUpdate
                                ,[SysD] UserCreate
                                ,[SysV] Version
                                ,[SysS] SysS
                            FROM [MD].[Users]
                            ";

            using var connection = _context.CreateConnection();
            var users = await connection.QueryAsync<User>(query);
            return users.ToList();
        }
        public async Task<User> GetUserById(string id)
        {
            var query = @"SELECT [I] Id
                                ,[C] UserName
                                ,[N] FullName
                                ,[ShortN] ShortName
                                ,[pwd] Password
                                ,[UType] UserType
                                ,[status] Status
                                ,[pwdexpireDay] PasswordExpireDay
                                ,[lastpwdchanged] LastPasswordChanged
                                ,[Company] CompanyId
                                ,[POS] PosId
                                ,[Tel] Tel
                                ,[Mobile] Mobile
                                ,[Addr] Address
                                ,[MailBox] Email
                                ,[ChatID] ChatId
                                ,[SysLoginInfo] SysLoginInfo
                                ,[SysU] UserUpdate
                                ,[SysD] UserCreate
                                ,[SysV] Version
                                ,[SysS] SysS
                            FROM [MD].[Users]
                            WHERE
	                        [C] = @Id ";

            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { id });
            return user;
        }

        public async Task<UserCompanyDto> GetUserByUserId(string userName)// pre: userId
        {
            var query = @"SELECT [I] Id
                                ,[C] UserName
                                ,[N] FullName
                                ,[ShortN] ShortName
                                ,[pwd] Password
                                ,[UType] UserType
                                ,[status] Status
                                ,[pwdexpireDay] PasswordExpireDay
                                ,[lastpwdchanged] LastPasswordChanged
                                ,[Company] CompanyId
                                ,(SELECT top 1 [N] FROM [MD].[T_M_Company] WHERE [I] = [Company]) CompanyName
                                ,[POS] PosId
                                ,[dbo].[fromAyDate]([birthday]) Birthday
                                ,[gender] Gender
                                ,[avatar] Avatar
                                ,[Tel] Tel
                                ,[Balance] Balance
                                ,[Mobile] Mobile
                                ,[Addr] Address
                                ,[MailBox] Email
                                ,[TaxCode] TaxCode
                                ,[DeviceId] DeviceId
                                ,[SysLoginInfo] SysLoginInfo
                                ,[SysU] UserUpdate
                                ,[SysD] UserCreate
                                ,[SysV] Version
                                ,[SysS] SysS
                                ,(SELECT DISTINCT TOP(1) [CP].[C]
                                    FROM [MD].[T_M_BossCompany] AS B
                                    INNER JOIN [MD].[USERS] AS U ON [B].[BossId] = U.[I]
                                    INNER JOIN [MD].[T_M_Company] AS CP ON [B].[CompanyId] = [CP].[I]
                                    WHERE [U].[C] = @userName AND [U].STATUS = 1 AND [B].[default] = 1) MapToCompany
                                ,(SELECT DISTINCT TOP(1) [BOSS].[C]
                                    FROM [MD].[T_M_BossReceiver] AS [B]
                                    INNER JOIN [MD].[USERS] AS [BOSS] ON [B].[BossId] = BOSS.[I]
                                    INNER JOIN [MD].[USERS] AS [RECEIVER] ON [B].[ReceiverId] = RECEIVER.[I]
                                    WHERE [RECEIVER].[C] = @userName AND [B].[default] = 1
                                    ) MapToBoss
                                ,(SELECT DISTINCT TOP(1) [V].[licensePlate]
                                    FROM [MD].[T_M_UserVehicle] AS Uv
                                    INNER JOIN [MD].[T_M_Vehicle] AS V ON [V].[I] = [Uv].[vehicleId]
                                    INNER JOIN [MD].[Users] AS U ON [U].[I] = [Uv].[userId]
                                    WHERE [U].[C] = @userName AND [Uv].[default] = 1) AS Vehicle
                            FROM [MD].[Users]
                            WHERE
	                        [C] = @userName ";
            var parameters = new DynamicParameters(new { username = userName });
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<UserCompanyDto>(query, new { userName });
                return user;
            }
        }

        public async Task UpdateUserPassword(string userId, string newPassword)
        {
            var query = "UPDATE MD.Users SET pwd = :password WHERE Lower(C) = :userId";

            var parameters = new DynamicParameters();
            parameters.Add("userId", userId.ToLower(), DbType.String);
            parameters.Add("password", newPassword, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<long> AddNew(UserAddNewParam param)
        {
            var procedureName = "sp_User_AddNew";
            var parameters = new DynamicParameters();

            parameters.Add("UserName", param.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("Name", param.FullName, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", param.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("Email", param.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("DeviceId", param.DeviceId, DbType.String, ParameterDirection.Input);

            using var connection = _context.CreateConnection();
            long id = await connection.QuerySingleOrDefaultAsync<long>(procedureName, parameters, commandType: CommandType.StoredProcedure);

            return id; //id:0 => username exists in database
        }

        public async Task<int> ChangePassword(ChangePasswordRequest param, long userId)
        {
            using var connection = _context.CreateConnection();
            string comparePWD = @"Select PWD From MD.Users Where I = :userId";
            string oldPwdHashed = await connection.QuerySingleOrDefaultAsync<string>(comparePWD, new { userId });
            var test = BUtils.GetMD5Hash(param.OldPassword + userId);
            if (!BUtils.CompareMD5Hash(oldPwdHashed.Trim(), param.OldPassword + userId)) return -1;

            string query = @"Update MD.Users Set PWD = :NewPassword Where I = :UserId";
            param.NewPassword = BUtils.GetMD5Hash(string.Concat(param.NewPassword, userId));

            return await connection.ExecuteAsync(query, new { param.NewPassword, userId });
        }

        public async Task<int> MapUserVehicle(MapUserVehicleRequest request)
        {
            using var conn = _context.CreateConnection();

            string queryGetVH = @"Select I From S900.Vehicles VH Where REGEXP_REPLACE(UPPER(VH.N), '[ .-]', '') = :VehicleNo";
            var vehicleId = await conn.QuerySingleOrDefaultAsync<long>(queryGetVH,
                new { VehicleNo = Regex.Replace(request.VehicleNo ?? "", @"[.\-\s]", "").ToUpper() });
            if (vehicleId == 0) return -1;

            var driverId = await conn.QuerySingleOrDefaultAsync<long>(
                "Select I From MD.Users Where C = :Username",
                new { request.Username });
            if (driverId == 0) return -2;

            var id = await SQLMethodHelper.GenarateId(conn);
            string code = await SQLMethodHelper.GenerateCodeWithPrefix(conn, string.Empty, "V2M");
            string query = @"Insert Into S900.V2M
                                (I, C, V, M, CustId, FromDate, SysU, SysD, SysV)
                            Select
                                :Id,
                                :Code,
                                :VehicleId,
                                (Select Mobile From MD.Users Where I = :DriverId),
                                :DriverId, 
                                :FromDate,
                                (Select I From MD.Users Where C = '900admin'),
                                :SysD,
                                1
                            From Dual";
            long fromDate = request.FromDate.ToAyDateTime();
            long SysD = localsetting.SQLgetDate();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Code", code);
            parameters.Add("VehicleId", vehicleId);
            parameters.Add("DriverId", driverId);
            parameters.Add("FromDate", fromDate);
            parameters.Add("SysD", SysD);
            return await conn.ExecuteAsync(query, parameters);
        }
    }
}