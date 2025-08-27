namespace IdentityServer.Models.Auth;

public class SendPinSMSRequest
{
    public string? NewPin { get; set; }
    public string? OldPin { get; set; }
}
