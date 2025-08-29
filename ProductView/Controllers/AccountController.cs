using Microsoft.AspNetCore.Mvc;
using ProductView.Services;
using ProductView.Helpers;
using System.Text;
using System.Text.Json;

namespace ProductView.Controllers
{
    [Route("Auth")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // 🔹 GET: Login
        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // 🔹 POST: Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            try
            {
                var token = await _authService.LoginAndGetTokenAsync(username, password);
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
            catch
            {
                TempData["Error"] = "Đăng nhập thất bại. Vui lòng thử lại.";
                return View();
            }
        }

        // 🔹 POST: Logout
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKeys.JwtToken);
            return RedirectToAction("Login");
        }

        // 🔹 GET: Register
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        // 🔹 POST: Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:62362/");

                var payload = new { username, password };

                var json = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("IdentityServer/v1/Auth/register", json);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập!";
                }
                else
                {
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<JsonElement>(content);

                        if (errorObj.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array && data.GetArrayLength() > 0)
                        {
                            ViewBag.Error = data[0].GetString();
                        }
                        else if (errorObj.TryGetProperty("message", out var message))
                        {
                            ViewBag.Error = message.GetString();
                        }
                        else
                        {
                            ViewBag.Error = "Đăng ký thất bại.";
                        }
                    }
                    catch
                    {
                        ViewBag.Error = "Đăng ký thất bại: " + content; // fallback
                    }
                }


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi đăng ký: " + ex.Message;
                return View();
            }
        }
    }
}
