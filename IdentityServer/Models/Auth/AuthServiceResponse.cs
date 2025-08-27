using IdentityServer.Domains.Dto;

namespace IdentityServer.Models.Auth
{
    public class AuthServiceResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public AuthServiceResponseData data { get; set; }
    }
    public class AuthServiceResponseData
    {     
        public string requestDate { get; set; }
        public string serviceName { get; set; }
        public int status { get; set; }
        public string token { get; set; }
    }
}
