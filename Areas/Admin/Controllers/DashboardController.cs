using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;
using Movie_management_system.Models;
using ModelUser = Movie_management_system.Models.User;

namespace Movie_management_system.Areas.Admin.Controllers
{
    [Area("Admin")]
    [JwtAuthFilter]
    public class DashboardController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        // ✅ Inject IConfiguration here
        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"]; // ✅ Now it works
        }
        #endregion

        #region Dashboard Index
        public async Task<IActionResult> Index()
        {

            List<MovieDTO> movies = new();
            List<ModelUser> users = new();
            List<Booking> bookings = new();

            try
            {
                var movieResponse = await _httpClient.GetAsync(_appBaseUrl + "MovieAPI");
                if (movieResponse.IsSuccessStatusCode)
                    movies = await movieResponse.Content.ReadFromJsonAsync<List<MovieDTO>>() ?? new List<MovieDTO>();

                var userResponse = await _httpClient.GetAsync(_appBaseUrl + "UserAPI");
                if (userResponse.IsSuccessStatusCode)
                    users = await userResponse.Content.ReadFromJsonAsync<List<ModelUser>>() ?? new List<ModelUser>();

                var bookingResponse = await _httpClient.GetAsync(_appBaseUrl + "BookingAPI");
                if (bookingResponse.IsSuccessStatusCode)
                    bookings = await bookingResponse.Content.ReadFromJsonAsync<List<Booking>>() ?? new List<Booking>();
                
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to load dashboard data.";
            }

            ViewBag.MovieCount = movies?.Count ?? 0;
            ViewBag.UserCount = users?.Count ?? 0;
            ViewBag.BookingCount = bookings?.Count ?? 0;
            ViewBag.TopMovies = movies?.Take(10).ToList() ?? new List<MovieDTO>();

            return View();
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            TokenManager.Token = null;
            Console.WriteLine("Admin logged out, token cleared.");
            return RedirectToAction("Index", "Login", new { area = "" });
        }
        #endregion

        #region EDIT USER GET
        [HttpGet]
        public async Task<IActionResult> EditUser()
        {

            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"UserAPI/{TokenManager.UserId}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserEditDTO>();
                    return View(user);
                }
                TempData["Error"] = "User not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching user: " + ex.Message);
                TempData["Error"] = "An error occurred while fetching the user.";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region EDIT USER POST
        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditDTO user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync(_appBaseUrl + $"UserAPI/{TokenManager.UserId}", user);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Your Profile has Updated Successfully!";
                        return RedirectToAction("Index", "Dashboard", new { area = "User" });
                    }
                    else
                    {
                        TempData["Error"] = "Unable to update your Profile!.";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating User: " + ex.Message);
                    TempData["Error"] = "An error occurred while updating the User.";
                }
            }

            return View(user);
        }
        #endregion

    }
}
