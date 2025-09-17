using Microsoft.AspNetCore.Mvc;
using Movie_management_system.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Movie_management_system.Helper;

public class RegisterController : Controller
{
    #region CONFIGURATION
    private readonly HttpClient _httpClient;
    private readonly string _appBaseUrl;

    public RegisterController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _appBaseUrl = configuration["AppBaseUrl"];
    }
    #endregion
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var registermodel = new RegisterDTO
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
            };

            var response = await _httpClient.PostAsJsonAsync(_appBaseUrl + "Auth/register", registermodel);
            var rawResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("Index", "Login");
            }

            TempData["Error"] = "Registration failed. Email might already be in use.";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during registration: " + ex.Message);
            TempData["Error"] = "An unexpected error occurred.";
        }

        return View(model);
    }
}
