namespace IdentityServer.Models;
public class BasicResponse
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
