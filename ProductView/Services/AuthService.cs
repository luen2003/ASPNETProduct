using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProductView.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://localhost:62362/"); // API IdentityServer
        }

        public async Task<string?> LoginAndGetTokenAsync(string username, string password, CancellationToken ct = default)
        {
            var payload = new
            {
                userName = username,
                password = password
            };

            var json = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.PostAsync("IdentityServer/v1/Auth/authService", json, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            Console.WriteLine("StatusCode: " + response.StatusCode);
            Console.WriteLine("Response: " + content);

            if (!response.IsSuccessStatusCode)
                return null;

            using var doc = JsonDocument.Parse(content);

            if (doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }

            return null;
        }
    }
}
