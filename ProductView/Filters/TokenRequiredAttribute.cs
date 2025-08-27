using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductView.Helpers;

namespace ProductView.Filters
{
    public class TokenRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrEmpty(token))
            {
                // Nếu chưa có token thì chuyển hướng về trang đăng nhập
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
