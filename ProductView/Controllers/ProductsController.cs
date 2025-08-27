using Microsoft.AspNetCore.Mvc;
using ProductView.Models;
using ProductView.Services;
using ProductView.Helpers; // để dùng SessionKeys

namespace ProductView.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        private string? GetToken()
        {
            return HttpContext.Session.GetString(SessionKeys.JwtToken);
        }

        // GET: Products
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var products = await _productService.GetProductsAsync(token, ct);
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto product, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(product);

            var result = await _productService.CreateProductAsync(product, token, ct);
            if (result != null)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Không thể tạo sản phẩm");
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id, token, ct);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto product, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(product);

            var success = await _productService.UpdateProductAsync(product, token, ct);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Không thể cập nhật sản phẩm");
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id, token, ct);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var success = await _productService.DeleteProductAsync(id, token, ct);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Không thể xóa sản phẩm");
            var product = await _productService.GetProductByIdAsync(id, token, ct);
            return View("Delete", product);
        }
    }
}
