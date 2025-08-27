namespace IdentityServer.Models.Auth
{
    public class CreateUserByPLXIDRequest
    {
        public string RequestId { get; set; }
        public string RequestDate { get; set; }
        public string PlxId { get; set; }
        public string? FullName { get; set; }
        public string OtpPhone { get; set; }
        public string MobilePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PIN { get; set; }
        public string CheckSum { get; set; }
    }
}
