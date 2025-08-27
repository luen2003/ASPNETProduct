using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ProductView.Models;

namespace ProductView.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        private void AddToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string token)
        {
            AddToken(token);
            var res = await _http.GetAsync("api/products");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                   ?? new List<ProductDto>();
        }

        public async Task<ProductDto?> GetProductAsync(string token, int id)
        {
            AddToken(token);
            var res = await _http.GetAsync($"api/products/{id}");
            if (!res.IsSuccessStatusCode) return null;
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateProductAsync(string token, ProductDto dto)
        {
            AddToken(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var res = await _http.PostAsync("api/products", content);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(string token, ProductDto dto)
        {
            AddToken(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var res = await _http.PutAsync($"api/products/{dto.Id}", content);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(string token, int id)
        {
            AddToken(token);
            var res = await _http.DeleteAsync($"api/products/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
