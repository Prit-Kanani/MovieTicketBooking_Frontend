using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;
using Movie_management_system.Models;

namespace Movie_management_system.Areas.Admin.Controllers
{
    [Area("Admin")]
    [JwtAuthFilter]
    public class ShowtimeController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public ShowtimeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region GET SHOWTIME INDEX
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            Relation.ScreenId = id;

            List<ShowtimeDTO> shows = new();

            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"ShowTimeAPI/{id}");
                if (response.IsSuccessStatusCode)
                    shows = await response.Content.ReadFromJsonAsync<List<ShowtimeDTO>>() ?? new List<ShowtimeDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching ShowTimes: " + ex.Message);
                TempData["Error"] = "Failed to load ShowTimes.";
            }

            return View(shows);
        }
        #endregion

        #region GET ADD OR EDIT SHOWTIME
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            // 1️⃣ Fetch movies for dropdown
            await PopulateMoviesDropdown();

            // 2️⃣ Add or Edit showtime
            if (id == null || id == 0)
            {
                return View(new ShowtimeAddDTO
                {
                    ShowId = id,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Time = TimeOnly.FromDateTime(DateTime.Now)
                });
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_appBaseUrl}ShowTimeAPI/Show/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // ✅ API returned success -> Deserialize to ShowtimeAddDTO
                    var showtime = await response.Content.ReadFromJsonAsync<ShowtimeAddDTO>()
                                                                    ?? new ShowtimeAddDTO();

                    return View(showtime);
                }
                else
                {
                    // ❌ API returned error -> Deserialize to APIResponse
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

                    TempData["Error"] = apiResponse?.Message ?? "Showtime not found.";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching showtime: " + ex.Message);
                TempData["Error"] = $"Failed to load showtime. {id}";
            }

            return RedirectToAction("Index", new { id = Relation.ScreenId });
        }
        #endregion

        #region POST ADD OR EDIT SHOWTIME
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ShowtimeAddDTO model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMoviesDropdown(); // repopulate before return
                TempData["Error"] = "Enter Valid Data!";
                return View(model);
            }

            // 🔒 Server-side validation
            if (model.Date < DateOnly.FromDateTime(DateTime.Today) ||
                (model.Date == DateOnly.FromDateTime(DateTime.Today) && model.Time <= TimeOnly.FromDateTime(DateTime.Now)))
            {
                await PopulateMoviesDropdown();
                ModelState.AddModelError("Date", "Showtime must be in the future.");
                ModelState.AddModelError("Time", "Showtime must be in the future.");
                return View(model);
            }

            if (model.Price < 0)
            {
                await PopulateMoviesDropdown();
                ModelState.AddModelError("Price", "Price cannot be negative.");
                return View(model);
            }

            HttpResponseMessage response;
            if (model.ShowId == null || model.ShowId == 0)
            {
                response = await _httpClient.PostAsJsonAsync($"{_appBaseUrl}ShowtimeAPI", model);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"{_appBaseUrl}ShowtimeAPI", model);
            }
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                string message = result?.Message ?? "No message returned";
                TempData["Success"] = message;
                return RedirectToAction("Index", new { id = Relation.ScreenId });
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                string errorMessage = error?.Message ?? "Something went wrong";
                TempData["Error"] = errorMessage;
            }
            

            Console.WriteLine("Error saving showtime: " + response.ReasonPhrase);
            
            await PopulateMoviesDropdown();
            return View(model);
        }
        #endregion

        #region POPULATEMOVIESDROPDOWN
        // 🔧 Helper method
        private async Task PopulateMoviesDropdown()
        {
            List<MovieDTO> movies = new();
            try
            {
                var movieResponse = await _httpClient.GetAsync($"{_appBaseUrl}MovieAPI");
                if (movieResponse.IsSuccessStatusCode)
                    movies = await movieResponse.Content.ReadFromJsonAsync<List<MovieDTO>>() ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movies: " + ex.Message);
            }
            ViewBag.Movies = movies;
        }

        #endregion

        #region DELETE SHOWTIME
        [HttpPost]
        public async Task<IActionResult> DeleteShowTimes([FromBody] List<int> ids)
        {
            try
            {
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ids),
                                                Encoding.UTF8, "application/json");
                Console.WriteLine("Deleting showtimes with IDs: " + string.Join(", ", ids));

                var response = await _httpClient.PostAsync($"{_appBaseUrl}ShowTimeAPI/DeleteShowTimes", content);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) 
                    {
                        TempData["Success"] = "All selected showtimes deleted successfully!";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                    {
                        TempData["Error"] = "Some showtimes deleted, some not found.";
                    }
                }
                else
                {
                    TempData["Error"] = "No showtimes could be deleted.";
                }
                return RedirectToAction("Index", new { id = Relation.ScreenId });
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error deleting showtimes: " + ex.Message);
                TempData["Error"] = "Failed to delete showtimes.";
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}
