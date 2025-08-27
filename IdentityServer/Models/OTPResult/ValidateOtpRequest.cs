namespace IdentityServer.Models.OTPResult;

public class ValidateOtpRequest
{
    public string OTPCode { get; set; } = null!;
    public string Username { get; set; } = null!;
}
