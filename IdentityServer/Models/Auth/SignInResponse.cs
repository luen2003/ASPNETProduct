using IdentityServer.Domains.Dto;

namespace IdentityServer.Models.Auth
{
    public class SignInResponse
    {
        public string code { get; set; } = null!;
        public string message { get; set; } = null!;
        public SignInResponseData? data { get; set; }
        public SignInResponse()
        {

        }
        public SignInResponse(string _code, string _message, SignInResponseData? _data)
        {
            code = _code;
            message = _message;
            data = _data;
        }
    }
    public class SignInResponseData
    {
        public string userName { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string requestDate { get; set; }
    }
}
