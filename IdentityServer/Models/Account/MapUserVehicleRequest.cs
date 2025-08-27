namespace IdentityServer.Models.Account;

public class MapUserVehicleRequest
{
    public string Username { get; set; } = null!;
    public string VehicleNo { get; set; } = null!;
    public string FromDate { get; set; } = null!;
}
