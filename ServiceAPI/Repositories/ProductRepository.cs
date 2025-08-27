using System.Data;
using Dapper;
using ServiceAPI.Context;
using ServiceAPI.Models;
using ServiceAPI.Repositories.Interfaces;

namespace ServiceAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            const string query = "SELECT Id, Name, Price FROM Products ORDER BY Id DESC";
            using var connection = _context.CreateConnection();
            var products = await connection.QueryAsync<Product>(query);
            return products.ToList();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            const string query = "SELECT Id, Name, Price FROM Products WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Product>(query, new { Id = id });
        }

        public async Task<Product?> AddAsync(Product product)
        {
            const string query = @"
                INSERT INTO Products (Name, Price)
                VALUES (@Name, @Price);";

            using var connection = _context.CreateConnection();
            var newId = await connection.ExecuteScalarAsync<int>(query, new { product.Name, product.Price });

            // Trả về product mới kèm Id
            product.Id = newId;
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            const string query = @"
                UPDATE Products 
                SET Name = @Name, Price = @Price 
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { product.Name, product.Price, product.Id });

            if (rowsAffected > 0)
            {
                return product;
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Products WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
