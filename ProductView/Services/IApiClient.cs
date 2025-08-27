using ProductView.Models;

namespace ProductView.Services
{
    public interface IApiClient
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(string token);
        Task<ProductDto?> GetProductAsync(string token, int id);
        Task<bool> CreateProductAsync(string token, ProductDto dto);
        Task<bool> UpdateProductAsync(string token, ProductDto dto);
        Task<bool> DeleteProductAsync(string token, int id);
    }
}
