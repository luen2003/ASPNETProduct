namespace ProductView.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;   // JWT token trả về
        public string RefreshToken { get; set; } = string.Empty; // nếu API có
        public string Message { get; set; } = string.Empty; // thông báo từ API
        public bool Success { get; set; }                   // true/false đăng nhập
    }
}
