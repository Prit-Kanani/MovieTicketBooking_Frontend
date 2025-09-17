using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using Movie_management_system.Helper;

public class JwtAuthFilter : TypeFilterAttribute
{
    public JwtAuthFilter() : base(typeof(JwtAuthFilterImpl)) { }
}

public class JwtAuthFilterImpl : IActionFilter
{
    private readonly IHttpClientFactory _httpClientFactory;

    public JwtAuthFilterImpl(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // 1️⃣ Redirect if token is missing
        if (string.IsNullOrEmpty(TokenManager.Token))
        {
            context.Result = new RedirectToActionResult("Index", "Login", new { area = "" });
            return;
        }

        // 2️⃣ Attach JWT token to HttpClient
        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenManager.Token);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
