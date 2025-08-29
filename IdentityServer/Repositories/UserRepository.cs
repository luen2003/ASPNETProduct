using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Helpers;
using IdentityServer.Contexts;
using IdentityServer.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DapperContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsers()
        {
            const string query = @"
                SELECT [I] Id, [C] UserName, [N] FullName, [ShortN] ShortName, [pwd] Password,
                       [UType] UserType, [status] Status, [pwdexpireDay] PasswordExpireDay,
                       [lastpwdchanged] LastPasswordChanged, [Company] CompanyId, [POS] PosId,
                       [Tel] Tel, [Mobile] Mobile, [Addr] Address, [MailBox] Email,
                       [SysLoginInfo] SysLoginInfo, [SysU] UserUpdate, [SysD] UserCreate,
                       [SysV] Version, [SysS] SysS
                FROM [MD].[Users]";

            using var connection = _context.CreateConnection();
            var users = await connection.QueryAsync<User>(query);
            return users.ToList();
        }

        public async Task<User> GetUserById(string id)
        {
            const string query = @"
                SELECT [I] Id, [C] UserName, [N] FullName, [ShortN] ShortName, [pwd] Password,
                       [UType] UserType, [status] Status, [pwdexpireDay] PasswordExpireDay,
                       [lastpwdchanged] LastPasswordChanged, [Company] CompanyId, [POS] PosId,
                       [Tel] Tel, [Mobile] Mobile, [Addr] Address, [MailBox] Email,
                       [ChatID] ChatId, [SysLoginInfo] SysLoginInfo, [SysU] UserUpdate,
                       [SysD] UserCreate, [SysV] Version, [SysS] SysS
                FROM [MD].[Users]
                WHERE [C] = @Id";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task<UserCompanyDto> GetUserByUserId(string userId)
        {
            const string query = @"
                SELECT u.[I] AS Id, u.[C] AS UserName, u.[N] AS FullName,
                       c.[I] AS CompanyId, c.[N] AS CompanyName
                FROM [MD].[Users] u
                LEFT JOIN [MD].[Company] c ON u.Company = c.I
                WHERE u.[C] = @userId";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<UserCompanyDto>(query, new { userId });
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            const string query = @"
                SELECT [I] Id,
                       [C] UserName,
                       [pwd] Password,
                       [Company] CompanyId,
                       [POS] PosId
                FROM [MD].[Users]
                WHERE [C] = @username";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { username });
        }

        public async Task UpdateUserPassword(string userId, string newPassword)
        {
            string newPwdHashed = BUtils.GetMD5Hash(newPassword + userId);

            const string query = @"UPDATE MD.Users SET PWD = @password WHERE LOWER(C) = @userId";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, new { userId = userId.ToLower(), password = newPwdHashed });
        }

        public async Task CreateUser(User user)
        {
            using var connection = _context.CreateConnection();

            string hashedPassword = BUtils.GetMD5Hash(user.Password);

            const string sql = @"
    INSERT INTO MD.Users (C, N, SHORTN, PWD, UTYPE)
    VALUES (@C, @N, @SHORTN, @PWD, @UTYPE)";

            await connection.ExecuteAsync(sql, new
            {
                C = user.UserName,
                N = user.FullName ?? user.UserName,
                SHORTN = user.ShortName ?? user.UserName,
                PWD = user.Password,
                UTYPE = user.UserType
            });

        }

        public async Task<long> AddNew(UserAddNewParam param)
        {
            using var connection = _context.CreateConnection();

            string hashedPassword = BUtils.GetMD5Hash(param.Password + param.UserName);

            var sql = @"
                INSERT INTO MD.Users (C, N, SHORTN, PWD, UTYPE, Status, SysD, SysU, SysV)
                VALUES (@C, @N, @SHORTN, @Password, @UType, 1, GETDATE(), @C, 1);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                C = param.UserName,
                N = param.FullName ?? param.UserName,
                SHORTN = param.ShortName ?? param.UserName,
                Password = hashedPassword,
                UType = param.UserType > 0 ? param.UserType : 1
            });
        }

        public async Task<int> ChangePassword(ChangePasswordRequest param, long userId)
        {
            string newPwdHashed = BUtils.GetMD5Hash(param.NewPassword + param.UserName);

            const string sql = @"UPDATE MD.Users SET PWD = @Password, LastPwdChanged = GETDATE() WHERE I = @userId";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Password = newPwdHashed, userId });
        }

        public async Task<int> MapUserVehicle(MapUserVehicleRequest request)
        {
            const string sql = @"
                INSERT INTO MD.UserVehicle (UserId, VehicleId, SysD, SysU, SysV)
                VALUES (@UserId, @VehicleId, GETDATE(), @UserId, 1)";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { request.UserId, request.VehicleId });
        }
    }
}
