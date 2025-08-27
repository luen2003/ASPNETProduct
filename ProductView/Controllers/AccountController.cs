using Microsoft.AspNetCore.Mvc;
using ProductView.Services;
using ProductView.Helpers; // để dùng SessionKeys

namespace ProductView.Controllers
{
    [Route("Auth")] // 👈 tất cả action trong controller này sẽ bắt đầu bằng /Auth
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            try
            {
                var token = await _authService.LoginAndGetTokenAsync(username, password);
                Console.WriteLine(token);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    HttpContext.Session.SetString(SessionKeys.JwtToken, token);
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Products");
                }

                TempData["Error"] = "Đăng nhập thất bại. Vui lòng kiểm tra tài khoản/mật khẩu.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi đăng nhập: " + ex.Message;
                return View();
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKeys.JwtToken);
            return RedirectToAction("Login");
        }
    }
}
