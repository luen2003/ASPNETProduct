using IdentityServer.Authentication;
using IdentityServer.Extensions;

namespace IdentityServer.Repositories
{
    public class ServiceUrlRepository : IServiceUrlRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger _logger;
        public ServiceUrlRepository(DapperContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<long> GetTimeUnlockPin()
        {
            var queryProductGroup =
                @"SELECT SYSVALUE FROM MD.SYSPARAMS WHERE C = '@TIMEUNLOCKPIN'";

            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<long>(queryProductGroup);
            }
        }

        public async Task<long> GetNumLockPin()
        {
            var queryProductGroup =
                @"SELECT SYSVALUE FROM MD.SYSPARAMS WHERE C = '@NUMLOCKPIN'";

            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<long>(queryProductGroup);
            }
        }

        public async Task<ServiceUrl> GetService(string code)
        {
            var queryQuotaServiceUrl = @"SELECT I Id
                                        ,C ServiceCode
                                        ,N ServiceName
                                        ,SHORTN ShortName
                                        ,SURL ServicePath
                                        ,STYPE ServiceType
                                        ,VAR1 Var1
                                        ,VAR2 Var2
                                        ,VAR3 Var3
                                        ,VAR4 Var4
                                        ,VAR5 Var5
                                        ,SUSTATUS Status
                                        ,SysU SysU
                                        ,SysD SysD
                                        ,SysV SysV
                                        ,SysS SysS                                
                                    FROM S900.SERVICEURL
                                    WHERE C = :code";

            using (IDbConnection connection = _context.CreateConnection())
            {
                var serviceUrl = await connection.QueryFirstOrDefaultAsync<ServiceUrl>(queryQuotaServiceUrl, new { code });
                return serviceUrl;
            }
        }
      
        public async Task<ServiceUrl?> GetServiceUrlByUserName(string userName)
        {
            var query = @"SELECT I Id
                                ,C ServiceCode
                                ,N ServiceName
                                ,SHORTN ShortName
                                ,SURL ServicePath
                                ,STYPE ServiceType
                                ,VAR1 Var1
                                ,VAR2 Var2
                                ,VAR3 Var3
                                ,VAR4 Var4
                                ,VAR5 PosId
                                ,SUSTATUS Status
                                ,SysU SysU
                                ,SysD SysD
                                ,SysV SysV
                                ,SysS Sys
                            FROM S900.SERVICEURL
                            WHERE VAR1 = :userName";
            var parameters = new DynamicParameters();
            parameters.Add("userName", userName, DbType.String);
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<ServiceUrl>(query, parameters);
            if (user != null)
            {
                string query2 = "Select I From MD.Users Where C = '900system'";
                var userId = await connection.QuerySingleOrDefaultAsync<long>(query2);
                user.UserId = userId;
            }
            return user;
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            var query = @"SELECT I Id
                                ,C UserName
                                ,N FullName
                                ,SHORTN ShortName
                                ,PWD Password
                                ,UTYPE UserType
                                ,PWDEXPIREDAY PasswordExpireDay
                                ,LASTPWDCHANGED AS LastPasswordChanged
                                ,COMPANY CompanyId
                                ,POS PosId
                                ,TEL Tel 
                                ,MOBILE Mobile
                                ,ADDR Address
                                ,MAILBOX Email
                                ,CHATID ChatId
                                ,SYSLOGININFO SysLoginInfo
                                ,SysU SysU
                                ,SysD SysD
                                ,SysV SysV
                                ,SysS SysS 
                            FROM MD.Users
                            WHERE LOWER(C) = LOWER(@userName)";
            var parameters = new DynamicParameters();
            parameters.Add("userName", userName, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);
                return user;
            }
        }

        public async Task<string> Portal_CreateUserAdmin(CreateUserByPLXIDRequest request)
        {
            var CheckRoleExisQuery = @"Select I From MD.UserGroup Where C = 'TQ01'";
            var InsertUsersQuery = @"Insert Into MD.Users(
                                         I,C,N,SHORTN,UTYPE,PWDEXPIREDAY,LASTPWDCHANGED AS LastPasswordChanged,
                                         COMPANY,POS,TEL,MOBILE,ADDR,MAILBOX,SYSLOGININFO,SysU,SysD,SysV) 
                                    SELECT  
                                        BOS.BASETAB_SEQ.nextval As I
                                        ,:PLXId
                                        ,:FullName
                                        ,'Cust-Admin'
                                        ,1
                                        ,1000
                                        ,:DateTimeNow
                                        ,(Select I From MD.Company Where C ='900')
                                        ,900999
                                        ,:OtpPhone
                                        ,:MobilePhone
                                        ,:Addr
                                        ,:Email
                                        ,5
                                        ,0
                                        ,:DateTimeNow
                                        ,1                                    
                                    FROM Dual";
            var InsertRole2UserQuery = @"Insert Into MD.User2Grp(I,C,U,G,SysU,SysD,SysV) 
                                        SELECT 
                                            BOS.BASETAB_SEQ.nextval As I
                                            ,900
                                            ,(Select I From MD.Users Where  C= :PLXId)
                                            ,:RoleId
                                            ,0
                                            ,:DateTimeNow
                                            ,1                                            
                                        FROM Dual";
            var InsertUserPINQuery = @"INSERT INTO S900.USERPIN (
                                            I, FK, PINCODE, PINSTATE, SYSU, SYSD, SYSV) 
                                       SELECT 
                                            BOS.BASETAB_SEQ.nextval As I
                                            ,(Select I From MD.Users Where C = :PLXId)
                                            ,:PIN
                                            ,1
                                            ,(Select I From MD.Users Where C = :PLXId)
                                            ,:DateTimeNow
                                            ,1                                            
                                       FROM Dual";

            var UpdateUsers = @"Update MD.USERS 
                                  SET 
                                     N = :FullName
                                    ,TEL = :OtpPhone
                                    ,MOBILE = :MobilePhone
                                    ,MAILBOX = :Email
                                    ,ADDR = :Addr
                                  WHERE C = :PLXId";

            using (IDbConnection connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowsAffected = 0;
                        var param = new DynamicParameters();
                        param.Add(name: "PLXId", request.PlxId, dbType: DbType.String);
                        param.Add(name: "FullName", request.FullName, dbType: DbType.String);
                        var roleId = await connection.QueryFirstOrDefaultAsync<long>(CheckRoleExisQuery);
                        var dateTimeNow = localsetting.SQLgetDate();
                        param.Add(name: "DateTimeNow", dateTimeNow, dbType: DbType.Int64);
                        param.Add(name: "OtpPhone", request.OtpPhone, dbType: DbType.String);
                        param.Add(name: "MobilePhone", request.MobilePhone, dbType: DbType.String);
                        param.Add(name: "Addr", request.Address, dbType: DbType.String);
                        param.Add(name: "Email", request.Email, dbType: DbType.String);
                        param.Add(name: "RoleId", roleId, dbType: DbType.Int64);
                        param.Add(name: "PIN", request.PIN, dbType: DbType.String);
                        try
                        {
                            rowsAffected = await connection.ExecuteAsync(InsertUsersQuery, param, transaction);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("unique constraint"))
                            {
                                rowsAffected = await connection.ExecuteAsync(UpdateUsers, param, transaction);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        if (roleId == 0)
                        {
                            transaction.Rollback();
                            return "Thất bại do Portal chưa tạo Role TQ01.";
                        }

                        try
                        {
                            rowsAffected = await connection.ExecuteAsync(InsertRole2UserQuery, param, transaction);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("unique constraint"))
                            {
                                rowsAffected = 1;
                            }
                            else
                            {
                                throw;
                            }
                        }
                        try
                        {
                            rowsAffected = await connection.ExecuteAsync(InsertUserPINQuery, param, transaction);
                        }
                        catch (Exception ex)
                        {

                            if (ex.Message.Contains("unique constraint"))
                            {
                                rowsAffected = 1;
                            }
                            else
                            {
                                throw;
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        connection.Close();
                        throw;
                    }

                }
                connection.Close();
            }
            return "";
        }

        public async Task<int> GetPickupPermissionByPosId(long posId)
        {
            const string query = @"Select cs.CSSTATE as PickupPermission 
                                   from S900.CUST2SRV cs 
                                   join S900.POS p on p.I = cs.CUSTID 
                                   join S900.SERVICEURL su on su.I = cs.SRVURLID
                                   where p.I in (select p.I from S900.POS p
                                                 start with p.I = :posId 
                                                 connect by prior p.PARENT = p.I)";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, new { posId });
            return result ?? -1;
        }

        public async Task<int> UpdatePickupPermissionByPosId(long posId, int permissionStatus)
        {
            const string query = @"Update S900.CUST2SRV 
                                   set CSSTATE = :permissionStatus
                                   where CUSTID = :posId";
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(query, new { posId, permissionStatus }, transaction);
                transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
                transaction.Rollback();
                return 0;
            }
        }

        public async Task<bool> AllowVoucherPermission(string plxId)
        {
            const string query = @"SELECT COALESCE(CUSTOMER,0) 
                                  FROM MD.USERPROFILE T0 
                                  INNER JOIN MD.USERS T1 ON T0.U=T1.I 
                                  WHERE T0.ISDEFAULT='1' AND 
                                        T1.C= :plxId";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, new { plxId });
            return result == 1;
        }

        public async Task<int> UpdateAllowVoucherPermission(long userId, bool status)
        {
            const string query = @"Update MD.USERPROFILE 
                                  set CUSTOMER = :status
                                  Where U = :userId";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { userId, status = status ? 1 : 0 });
        }

        public async Task<List<NotificationSetting>> GetListNotificationSetting()
        {
            const string query = @"Select ns.I as Id, ns.N as NotificationName
                                   from MD.NOTIFICATIONSETTING ns";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<NotificationSetting>(query);
            return result.ToList();
        }

        public async Task<List<NotificationSetting>> GetNotificationSettingByUser(long userId)
        {
            const string query = @"Select ns.I as Id, 
                                          ns.N as NotificationName, 
                                          (CASE 
                                               WHEN n2u.status is null or n2u.status = 1 THEN 1 ELSE 0
                                          END) as Status
                                   from MD.NOTI2U n2u 
                                   join MD.NOTIFICATIONSETTING ns on n2u.ns = ns.I
                                   where  n2u.U = :userId";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<NotificationSetting>(query, new { userId });
            return result.ToList();
        }

        public async Task<bool> IsUserNotificationExists(long notificationId, long userId)
        {
            const string query = @"Select count(n2u.I) from MD.NOTI2U n2u 
                                   where n2u.u = :userId and n2u.ns = :notificationId";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<int>(query, new { userId, notificationId });
            return result == 1;
        }

        public async Task<bool> CreateUserNotificationSetting(long notificationId, long userId, bool status)
        {
            const string query = @"Insert into MD.NOTI2U(I, U, NS, STATUS)
                                   SELECT  BOS.TRX_SEQ.nextval As I, 
                                           :userId, 
                                           :notificationId, 
                                           :status
                                   FROM Dual";
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, new { userId, notificationId, status = status ? 1 : 0 });
            return result == 1;
        }

        public async Task<bool> UpdateUserNotificationSetting(long notificationId, long userId, bool status)
        {
            const string query = @"Update MD.NOTI2U
                                   set status = :status
                                   where U = :userId and NS = :notificationId";
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, new { userId, notificationId, status = status ? 1 : 0 });
            return result == 1;
        }

        public async Task<CheckPinDto> GetLastLogPin(CheckPinRequest request, long UserPinId, long dateTimeNow, long timeUnlockPin)
        {
            var query = @"SELECT I
                                ,PLXID
                                ,VEHICLENO
                                ,COMPANY
                                ,POS
                                ,TXNID
                                ,TID
                                ,PINCODE
                                ,PINSTATE
                                ,VAR1 COUNTFALSE
                                ,LOCKDATE
                                ,MSGTEXT
                                ,PINSTATE
                                ,REGDATE
                        FROM S900.LOGPIN
                        WHERE 
			                    PLXID = :PLXID AND 
			                    FK = :UserPINId AND
			                    COMPANY = :Company AND
			                    POS = :POS AND
			                    TXNID = :TXNID AND	
                                :dateTimeNow <= SYSD + 60 * :timeUnlockPin 
                        ORDER BY SYSD DESC";
            var param = new DynamicParameters();
            param.Add(name: "PLXID", request.PlxId, dbType: DbType.String);
            param.Add(name: "UserPINId", UserPinId, dbType: DbType.Int64);
            param.Add(name: "Company", request.Company, dbType: DbType.String);
            param.Add(name: "POS", request.POS, dbType: DbType.String);
            param.Add(name: "TXNID", request.TXNID, dbType: DbType.String);
            param.Add(name: "dateTimeNow", dateTimeNow, dbType: DbType.String);
            param.Add(name: "timeUnlockPin", timeUnlockPin, dbType: DbType.String);
            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<CheckPinDto>(query, param);
            }
        }

        public async Task<long> MaxLockDate(string PLXID, long UserPinId)
        {
            var query = @"SELECT COALESCE(Max(LOCKDATE),0) LOCKDATE
                                      FROM S900.LOGPIN
                                      WHERE PLXID = :PLXID AND FK = :UserPinId";
            var param = new DynamicParameters();
            param.Add(name: "PLXID", PLXID, dbType: DbType.String);
            param.Add(name: "UserPinId", UserPinId, dbType: DbType.Int64);
            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<int>(query, param);
            }
        }

        public async Task<long> CountLogPinFalse(CheckPinRequest request, long userPinId, long maxLockDate)
        {
            var query = @"SELECT COUNT(I)
                        FROM S900.LOGPIN
                        WHERE PLXID = :PLXID AND 
			                        FK = :UserPINId AND
			                        COMPANY = :Company AND
			                        POS = :POS AND
			                        TXNID = :TXNID AND
			                        RESDATE IS NULL AND
                                    LOCKDATE = 0";
            var param = new DynamicParameters();
            param.Add(name: "PLXID", request.PlxId, dbType: DbType.String);
            param.Add(name: "UserPINId", userPinId, dbType: DbType.Int64);
            param.Add(name: "Company", request.Company, dbType: DbType.String);
            param.Add(name: "POS", request.POS, dbType: DbType.String);
            param.Add(name: "TXNID", request.TXNID, dbType: DbType.String);
            param.Add(name: "TID", request.TID, dbType: DbType.String);
            param.Add(name: "MaxLockDate", maxLockDate, dbType: DbType.Int64);

            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<long>(query, param);
            }
        }

        public async Task<UserPin> GetPINByPLXId(string code)
        {
            var SelectPINQuery = @"SELECT 
                                    U.C PlxId,                                   
                                    USERPIN.I Id,
                                    USERPIN.C Code,
                                    USERPIN.N Name,
                                    USERPIN.PINCODE Pin,
                                    USERPIN.PINSTATE State,
                                    U.Tel OtpPhone
                                   FROM S900.USERPIN
                                   LEFT JOIN MD.USERS U ON U.I = USERPIN.FK
                                   WHERE U.C = :code";

            using (IDbConnection connection = _context.CreateConnection())
            {
                var userPin = await connection.QueryFirstOrDefaultAsync<UserPin>(SelectPINQuery, new { code });
                return userPin;
            }
        }

        public async Task<UserPin> GetPINByVehicleNo(string code, string plxId)
        {
            var SelectPINQuery = @"SELECT 
		                                    U.C PlxId,
		                                    USERPIN.I Id,
		                                    USERPIN.C Code,
		                                    USERPIN.N Name,
		                                    USERPIN.PINCODE Pin,
		                                    USERPIN.PINSTATE State
                                    From S900.Vehicles VH
                                    Inner Join(
	                                    Select V2M.V, V2M.M, V2M. CustId As Driver
	                                    From S900.V2M 
	                                    Inner Join(
		                                    Select V, Max(FromDate) As MaxD
		                                    From S900.V2M
		                                    Where FromDate <= :DateTimeNow
		                                    Group By V
	                                    )SubMax On V2M.V=SubMax.V And V2M.FromDate = SubMax.MaxD
                                    )T2 On VH.I=T2.V
                                    INNER JOIN MD.USERS U ON U.I = T2.Driver
                                    INNER JOIN S900.USERPIN ON USERPIN.FK = U.I
                                    LEFT Join S900.POS ON VH.CustID = POS.I AND POS.RefCode = :PLXID 
                                    Where REGEXP_REPLACE(UPPER(VH.N), '[ .-]', '') = :vehicleNo";

            using (IDbConnection connection = _context.CreateConnection())
            {
                var param = new DynamicParameters();
                param.Add(name: "PLXID", plxId, dbType: DbType.String);
                param.Add(name: "VehicleNo", Regex.Replace(code ?? "", @"[.\-\s]", "").ToUpper(), dbType: DbType.String);
                param.Add(name: "DateTimeNow", localsetting.SQLgetDate(), dbType: DbType.String);
                var userPin = await connection.QueryFirstOrDefaultAsync<UserPin>(SelectPINQuery, param);
                return userPin;
            }
        }
        public async Task<List<CheckPinDto>> SelectLogPin(CheckPinRequest request)
        {
            var SelectLogPin = @"SELECT 
                                    I
                                    ,PLXID
                                    ,VEHICLENO
                                    ,COMPANY
                                    ,POS
                                    ,TXNID
                                    ,TID
                                    ,PINCODE
                                    ,MSGTEXT
                                    ,PINSTATE
                                    ,REGDATE
                                FROM S900.LOGPIN
                                WHERE 
                                    (PLXID = :PLXID OR VEHICLENO = :VehicleNo) AND
		                            COMPANY = :Company AND
		                            POS = :POS AND
		                            TXNID = :TXNID AND
		                            PINCODE = :PINCODE";
            var param = new DynamicParameters();
            param.Add(name: "PLXID", request.PlxId, dbType: DbType.String);
            param.Add(name: "VehicleNo", request.VehicleNo, dbType: DbType.String);
            param.Add(name: "Company", request.Company, dbType: DbType.String);
            param.Add(name: "POS", request.POS, dbType: DbType.String);
            param.Add(name: "TXNID", request.TXNID, dbType: DbType.String);
            param.Add(name: "TID", request.TID, dbType: DbType.String);
            param.Add(name: "PINCODE", request.PIN, dbType: DbType.String);
            using (IDbConnection connection = _context.CreateConnection())
            {
                var listLogPin = await connection.QueryAsync<CheckPinDto>(SelectLogPin, param);
                return listLogPin.ToList();
            }
        }

        public async Task<int> InsertLogPin(CheckPinRequest request, UserPin userPin, String msgText, long lockDate, long PINState, long CountFalse)
        {
            var InsertLogPin = @"INSERT INTO S900.LOGPIN 
	                              (I, FK, PLXID, VEHICLENO, COMPANY, POS, TXNID, TID, PINCODE, PININPUT, MSGTEXT, REGDATE, LOCKDATE, PINSTATE, VAR1, SYSU, SYSD, SYSV) 
                                  SELECT 
                                    BOS.BASETAB_SEQ.nextval As I
                                    ,:FK
                                    ,:PLXID
                                    ,:VehicleNo
                                    ,:Company
                                    ,:POS
                                    ,:TXNID
                                    ,:TID
                                    ,:PINCODE
                                    ,:PININPUT
                                    ,:MsgText
                                    ,:RegDate
                                    ,:LockDate
                                    ,:PinState
                                    ,:CountFalse
                                    ,0
                                    ,:DateTimeNow
                                    ,1                                                                                
                                  FROM Dual";
  
            var DateTineNow = localsetting.SQLgetDate();
            var param = new DynamicParameters();
            param.Add(name: "FK", userPin.Id, dbType: DbType.Int64);
            param.Add(name: "PLXID", request.PlxId, dbType: DbType.String);
            param.Add(name: "VehicleNo", request.VehicleNo, dbType: DbType.String);
            param.Add(name: "Company", request.Company, dbType: DbType.String);
            param.Add(name: "POS", request.POS, dbType: DbType.String);
            param.Add(name: "TXNID", request.TXNID, dbType: DbType.String);
            param.Add(name: "TID", request.TID, dbType: DbType.String);
            param.Add(name: "PINCODE", userPin.Pin, dbType: DbType.String);
            param.Add(name: "PININPUT", request.PIN, dbType: DbType.String);
            param.Add(name: "MsgText", msgText, dbType: DbType.String);
            param.Add(name: "RegDate", DateTineNow, dbType: DbType.Int64);
            param.Add(name: "LockDate", lockDate, dbType: DbType.Int64);
            param.Add(name: "PinState", PINState, dbType: DbType.Int64);
            param.Add(name: "CountFalse", CountFalse, dbType: DbType.Int64);
            param.Add(name: "DateTimeNow", DateTineNow, dbType: DbType.Int64);

            using (IDbConnection connection = _context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(InsertLogPin, param);
                //rowsAffected = await connection.ExecuteAsync(updateLogPin, param);
                return rowsAffected;
            }
        }

        public async Task<int> UpdatePin(UserPin userPin)
        {
            var UpdateUserPin = "Update S900.USERPIN SET PINCODE = :PinCode WHERE I = :Id";
            var param = new DynamicParameters();
            param.Add(name: "PinCode", userPin.Pin, dbType: DbType.String);
            param.Add(name: "Id", userPin.Id, dbType: DbType.Int64);
            using (IDbConnection connection = _context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(UpdateUserPin, param);
                return rowsAffected;
            }
        }

        public async Task<int> UpdateUsers(CreateUserByPLXIDRequest request)
        {
            var UpdateUsers = @"Update MD.USERS 
                                  SET 
                                     N = :FullName
                                    ,TEL = :MobileNumber
                                    ,MOBILE = :PhoneNumber
                                    ,MAILBOX = :Email
                                    ,ADDR = :Address
                                  WHERE I = :Id";
            var param = new DynamicParameters();
            param.Add(name: "FullName", request.FullName, dbType: DbType.String);
            param.Add(name: "MobileNumber", request.OtpPhone, dbType: DbType.String);
            param.Add(name: "PhoneNumber", request.MobilePhone, dbType: DbType.String);
            param.Add(name: "Email", request.Email, dbType: DbType.String);
            param.Add(name: "Address", request.Address, dbType: DbType.String);
            using (IDbConnection connection = _context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(UpdateUsers, param);
                return rowsAffected;
            }
        }
        public async Task<int> InsertSMSLog(LogSMS request)
        {
            var queryInsert = @"INSERT INTO S900.LOGSMS(I,PHONENUMBER,REQDATE,VAR2,VAR3,SYSD,SYSV) 
                                SELECT  BOS.TRX_SEQ.nextval As I 
                                       ,:PhoneNumber
                                       ,:TransDate
                                       ,:RequestStr
                                       ,:ResponseStr
                                       ,:SysD
                                       ,1
                               FROM Dual";

            using (IDbConnection connection = _context.CreateConnection())
            {
                var param = new DynamicParameters();
                param.Add(name: "PhoneNumber", request.PhoneNumber, dbType: DbType.String);
                param.Add(name: "TransDate", request.TransDate, DbType.Int64);
                param.Add(name: "RequestStr", request.RequestStr, DbType.String);
                param.Add(name: "ResponseStr", request.ResponseStr, DbType.String);
                param.Add(name: "SysD", localsetting.SQLgetDate(), DbType.Int64);
                return await connection.ExecuteAsync(queryInsert, param);
            }
        }

        public async Task<(int Status, string PlxId, string PhoneNumber)> InsertUserPin(string pinCode, ApplicationUser applicationUser, string? oldPin = null)
        {
            string queryGetUserPin = @"Select PINCode From S900.UserPIN Where FK= :userId --And (:OldPin is null Or PinCode = :OldPin)";
            string queryUpdate = @"Update S900.UserPIN Set PINCODE = :pinCode Where Fk = :userId";
            string queryUpdateLockDate = @"Update S900.LOGPIN Set LOCKDATE = 0 Where FK= (Select Distinct I From S900.UserPIN Where FK= :userId)";
            string queryInsert = @"INSERT INTO S900.USERPIN (
                                            I, C, FK, PINCODE, SysU, SysD) 
                                       SELECT 
                                            BOS.BASETAB_SEQ.nextval As I
                                            ,:pinCode
                                            ,:userId
                                            ,:pinCode
                                            ,:DateTimeNow
                                            ,1                                            
                                       FROM Dual";
            string queryGetUserInfo = @"Select C, Coalesce(TEL,MOBILE) As Phone From MD.Users Where I = :userId";
            using IDbConnection conn = _context.CreateConnection();
            var currPin = await conn.QuerySingleOrDefaultAsync<string>(queryGetUserPin, new { applicationUser.UserId, oldPin });
            bool result = false;
            if (string.IsNullOrEmpty(currPin))
            {
                result = await conn.ExecuteAsync(queryInsert, new { pinCode, applicationUser.UserId, DateTimeNow = localsetting.SQLgetDate() }) > 0;
            }
            else
            {
                if (currPin == oldPin)
                {
                    result = await DapperExtension.ExecuteTransactionAsync(conn, async transaction =>
                    {
                        await conn.ExecuteAsync(queryUpdate, new { pinCode, applicationUser.UserId }, transaction);
                        await conn.ExecuteAsync(queryUpdateLockDate, new { applicationUser.UserId }, transaction);
                    });
                }
                else
                    return (-1, "", "");
            }

            if (result)
            {
                var (plxId, phone) = await conn.QuerySingleOrDefaultAsync<(string, string)>(queryGetUserInfo, new { applicationUser.UserId });
                return (1, plxId, phone);
            }

            return (0, "", "");
        }
    }
}