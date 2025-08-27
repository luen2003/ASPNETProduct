namespace IdentityServer.Models.Account
{
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string CheckSum { get; set; } = null!;
    }
}
