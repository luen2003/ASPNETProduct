namespace IdentityServer.Models.Account
{
    public class SignUpResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
