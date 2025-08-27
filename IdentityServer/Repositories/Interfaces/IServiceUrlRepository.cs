using IdentityServer.Authentication;

namespace IdentityServer.Repositories.Interfaces
{
    public interface IServiceUrlRepository
    {
        Task<ServiceUrl?> GetServiceUrlByUserName(string userName);
        Task<long> GetTimeUnlockPin();
        Task<long> GetNumLockPin();
        Task<ServiceUrl> GetService(string code);
        Task<User> GetUserByUserName(string userName);
        Task<CheckPinDto> GetLastLogPin(CheckPinRequest request, long UserPinId, long dateTimeNow, long timeUnlockPin);
        Task<long> MaxLockDate(string PLXID, long UserPinId);
        Task<UserPin> GetPINByPLXId(string code);
        Task<UserPin> GetPINByVehicleNo(string code, string plxId);
        Task<long> CountLogPinFalse(CheckPinRequest request, long userPinId, long maxLockDate);
        Task<int> UpdatePin(UserPin pin);
        Task<int> UpdateUsers(CreateUserByPLXIDRequest request);
        Task<int> InsertLogPin(CheckPinRequest request, UserPin userPin, String msgText, long lockDate, long PINState, long CountFalse);
        Task<int> InsertSMSLog(LogSMS request);
        Task<List<CheckPinDto>> SelectLogPin(CheckPinRequest request);
        Task<string> Portal_CreateUserAdmin(CreateUserByPLXIDRequest param);
        Task<int> GetPickupPermissionByPosId(long posId);
        Task<int> UpdatePickupPermissionByPosId(long posId, int permissionStatus);
        Task<bool> AllowVoucherPermission(string plxId);
        Task<int> UpdateAllowVoucherPermission(long userId, bool status);
        Task<List<NotificationSetting>> GetListNotificationSetting();
        Task<List<NotificationSetting>> GetNotificationSettingByUser(long userId);
        Task<bool> IsUserNotificationExists(long notificationId, long userId);
        Task<bool> CreateUserNotificationSetting(long notificationId, long userId, bool status);
        Task<bool> UpdateUserNotificationSetting(long notificationId, long userId, bool status);
        Task<(int Status, string PlxId, string PhoneNumber)> InsertUserPin(string pinCode, ApplicationUser applicationUser, string? oldPin = null);
    }
}
