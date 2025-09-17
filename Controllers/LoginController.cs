using Microsoft.AspNetCore.Mvc;
using Movie_management_system.Models;
using Movie_management_system.Helper;
using System.Text.Json;

public class LoginController : Controller
{
    #region CONFIGURATION
    private readonly HttpClient _client;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("ApiClient");
    }
    #endregion

    #region LOGIN PAGE
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {

            var response = await _client.PostAsJsonAsync("Auth/login", model);

            var rawResponse = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse();
                TokenManager.Token = result.Token;
                TokenManager.Role = result.Role;
                TokenManager.Email = result.Email;
                TokenManager.UserId = result.UserId;
                TokenManager.Name = result.Name;
                TempData["Success"] = result.Message;
                if (result.Role == "Admin") 
                {
                    
                    return Redirect("/Admin/Dashboard");
                }
                else 
                {
                    return Redirect("/User/Dashboard");
                }
            }
            TempData["Error"] = "Invalid email or password.";
        }
        catch (Exception)
        {
            TempData["Error"] = "An unexpected error occurred.";
        }

        return View(model);
    }
    #endregion

}
