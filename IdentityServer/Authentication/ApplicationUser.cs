namespace IdentityServer.Authentication;
public class ApplicationUser
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ServiceCode { get; set; } = string.Empty;
    public long PosId { get; set; }
    public string PlxId { get; set; } = string.Empty;
}
