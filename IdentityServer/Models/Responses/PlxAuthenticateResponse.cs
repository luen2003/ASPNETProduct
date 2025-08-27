namespace IdentityServer.Models.Responses;

public class PlxAuthenticateResponse
{
    public string ResponseId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public AuthPLXData Data { get; set; } = null!;
    public string CheckSum { get; set; } = null!;
    public string StatusCode { get; set; } = null!;
    public string ResponseDate { get; set; } = null!;
}

public class AuthPLXData
{
    public string Address { get; set; } = null!;
    public string Sex { get; set; } = null!;
    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string PlxPwd { get; set; } = null!;
    public string Birthday { get; set; } = null!;
    public string OtpPhone { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string AccountStatus { get; set; } = null!;
    public string PlxId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public int Type { get; set; }
    public string MobileNumber { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public long RefId { get; set; }
    public long PortalId { get; set; }
}

