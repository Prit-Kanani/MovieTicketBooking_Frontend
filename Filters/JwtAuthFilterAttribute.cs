using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movie_management_system.Helper;

namespace Movie_management_system.Filters
{
    // Use this attribute like: [JwtAuthFilter]
    public class JwtAuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Redirect to login if there's no token
            if (string.IsNullOrEmpty(TokenManager.Token))
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { area = "" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
