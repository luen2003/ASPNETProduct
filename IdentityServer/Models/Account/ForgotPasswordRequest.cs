namespace IdentityServer.Models.Account;

public class ForgotPasswordRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
