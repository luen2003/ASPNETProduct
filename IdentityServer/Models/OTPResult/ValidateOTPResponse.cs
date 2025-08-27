namespace IdentityServer.Models.OTPResult;

public class ValidateOTPResponse
{
    public string ResponseId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string CheckSum { get; set; } = null!;
    public string StatusCode { get; set; } = null!;
    public string ResponseDate { get; set; } = null!;
}
