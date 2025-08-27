using ProductView.Models;

namespace ProductView.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync(string token, CancellationToken ct = default);
        Task<ProductDto?> GetProductByIdAsync(int id, string token, CancellationToken ct = default);
        Task<ProductDto?> CreateProductAsync(ProductDto product, string token, CancellationToken ct = default);
        Task<bool> UpdateProductAsync(ProductDto product, string token, CancellationToken ct = default);
        Task<bool> DeleteProductAsync(int id, string token, CancellationToken ct = default);
    }
}
