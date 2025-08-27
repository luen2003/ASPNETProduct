namespace IdentityServer.Domains.Dto;

public class CustomerInfoDto
{
    public long CustId { get; set; }
    public string CustCode { get; set; } = null!;
    public string CustName { get; set; } = null!;
    public string Addr { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNo { get; set; } = null!;
    public string Tel { get; set; } = null!;
    public string TaxCode { get; set; } = null!;
    public string RefCode { get; set; } = null!;
    public string ParentId { get; set; } = null!;
}

public class CustInfoEgasAPIResponse
{
    public string ResponseStatus { get; set; } = null!;
    public CustomerInfoDto CustData { get; set; } = null!;
}