using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Models;
using ServiceAPI.Models.Dtos;
using ServiceAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceAPI.Controllers
{
    [ApiController]
    [Route("/ServiceAPI/v1/[controller]")]
    [Authorize] // Bắt buộc có token JWT để truy cập tất cả action
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
        {
            try
            {
                var products = await _repository.GetAllAsync();

                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                });

                return ApiResponse<IEnumerable<ProductDto>>.Ok(productDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductDto>>.Fail("Lỗi khi lấy danh sách sản phẩm.", ex.Message);
            }
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return ApiResponse<ProductDto>.NotFound("Không tìm thấy sản phẩm.");

                var dto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };

                return ApiResponse<ProductDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail("Lỗi khi lấy chi tiết sản phẩm.", ex.Message);
            }
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductDto productDto)
        {
            try
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price
                };

                var created = await _repository.AddAsync(product);

                var resultDto = new ProductDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Price = created.Price
                };

                return ApiResponse<ProductDto>.Ok(resultDto, "Thêm sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail("Lỗi khi thêm sản phẩm.", ex.Message);
            }
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, UpdateProductDto productDto)
        {
            try
            {
                var existingProduct = await _repository.GetByIdAsync(id);
                if (existingProduct == null)
                    return ApiResponse<ProductDto>.NotFound("Không tìm thấy sản phẩm để cập nhật.");

                existingProduct.Name = productDto.Name;
                existingProduct.Price = productDto.Price;

                var updated = await _repository.UpdateAsync(existingProduct);

                var resultDto = new ProductDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Price = updated.Price
                };

                return ApiResponse<ProductDto>.Ok(resultDto, "Cập nhật sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDto>.Fail("Lỗi khi cập nhật sản phẩm.", ex.Message);
            }
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _repository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<object>.NotFound("Không tìm thấy sản phẩm để xóa.");

                return ApiResponse<object>.Ok(null, "Xóa sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("Lỗi khi xóa sản phẩm.", ex.Message);
            }
        }
    }
}
