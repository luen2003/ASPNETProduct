namespace IdentityServer.Models.Auth
{
    public class LogSMS
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public long TransDate { get; set; }
        public string RequestStr { get; set; } = string.Empty;
        public string ResponseStr { get; set; } = string.Empty;
    }
}
