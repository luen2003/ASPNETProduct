using Microsoft.AspNetCore.Mvc;

namespace ProductView.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
            return View(model: exception?.Message);
        }

    }
}
