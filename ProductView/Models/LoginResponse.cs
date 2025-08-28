namespace ProductView.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty; 
        public string RefreshToken { get; set; } = string.Empty; 
        public string Message { get; set; } = string.Empty; 
        public bool Success { get; set; }                  
    }
}
