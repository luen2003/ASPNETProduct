namespace IdentityServer.Domains.Dto;

public class PlxIdInfoDto
{
    public string StatusCode { get; set; } = null!;
    public string Msg { get; set; } = null!;
    public string PlxId { get; set; } = null!;
    public string PlxPwd { get; set; } = null!;
    public string AccessTokenn { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public int PlxType { get; set; }
    public string MobileNumbe { get; set; } = null!;
    public string OptPhone { get; set; } = null!;
    public string EmailAddres { get; set; } = null!;
    public DateTime ResponseDate { get; set; }
    public string PortalId { get; set; } = null!;
    public string RefId { get; set; } = null!;
}
