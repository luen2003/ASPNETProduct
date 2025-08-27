namespace IdentityServer.Domains.Dto;

public class UserSetting
{
    public int PickupPermissionDto { get; set; }
    public bool AllowVoucherPermission { get; set; }
    public List<NotificationSetting> NotificationSettings { get; set; }
}

public class NotificationSetting
{
    public long Id { get; set; }
    public string NotificationName { get; set; }
    public bool Status { get; set; }
}
