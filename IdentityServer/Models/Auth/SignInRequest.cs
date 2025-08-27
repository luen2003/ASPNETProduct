namespace IdentityServer.Models.Auth
{
    public class SignInRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int AuthenTyp { get; set; }
        public Boolean RememberMe { get; set; }
    }
}
