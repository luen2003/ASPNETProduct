namespace IdentityServer.Models.Auth
{
    public class AuthServiceRequest
    {
        public string? RequestId { get; set; }
        public string? RequestDate { get; set; }
        public string? CheckSum { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
