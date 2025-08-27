namespace IdentityServer.Models.Auth;

public class RefreshBothTokenRequest
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
