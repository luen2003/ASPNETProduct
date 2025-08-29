using IdentityServer.Authentication;
using IdentityServer.Services;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Route("IdentityServer/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Tạo JWT token từ username/password
        /// </summary>
        [HttpPost("authService")]
        public async Task<IActionResult> AuthService([FromBody] AuthServiceRequest param)
        {
            try
            {
                var response = await _authService.AuthCreateToken(param);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthService error");
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }

        /// <summary>
        /// Test API
        /// </summary>
        [HttpPost("TestAPI")]
        public IActionResult GetDataTest()
        {
            return Ok("Responsetest");
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    try
    {
        var user = await _authService.RegisterUserAsync(request);
        if (user == null)
        {
            return BadRequest("Username đã tồn tại.");
        }

        return Ok(new
        {
            message = "Đăng ký thành công.",
            username = user.UserName
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Đăng ký thất bại.");
        // Tạm thời trả về chi tiết để debug (chỉ dùng dev)
        return StatusCode(500, $"Lỗi hệ thống: {ex.Message} - {ex.StackTrace}");
    }
}

    }
}
