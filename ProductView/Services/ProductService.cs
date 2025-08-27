using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ProductView.Models;

namespace ProductView.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://localhost:7092/"); // Base address API
        }

        // GET all products
        public async Task<List<ProductDto>> GetProductsAsync(string token, CancellationToken ct = default)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync("ServiceAPI/v1/Product", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine("API Response JSON: " + json);

            var result = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<ProductDto>();
        }

        // GET product by id
        public async Task<ProductDto?> GetProductByIdAsync(int id, string token, CancellationToken ct = default)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync($"ServiceAPI/v1/Product/{id}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine("API Response JSON: " + json);

            // Nếu API trả về 1 object
            var result = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data;
        }

        // CREATE new product
        public async Task<ProductDto?> CreateProductAsync(ProductDto product, string token, CancellationToken ct = default)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("ServiceAPI/v1/Product", content, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine("API Response JSON: " + json);

            var result = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data;
        }

        // UPDATE product
        public async Task<bool> UpdateProductAsync(ProductDto product, string token, CancellationToken ct = default)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"ServiceAPI/v1/Product/{product.Id}", content, ct);

            return response.IsSuccessStatusCode;
        }

        // DELETE product
        public async Task<bool> DeleteProductAsync(int id, string token, CancellationToken ct = default)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.DeleteAsync($"ServiceAPI/v1/Product/{id}", ct);
            return response.IsSuccessStatusCode;
        }

        
    }
}
