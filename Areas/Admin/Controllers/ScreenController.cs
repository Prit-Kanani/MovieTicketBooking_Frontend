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
    public class ScreenController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public ScreenController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region GET SCREENS BY THEATRE
        [HttpGet]
        public async Task<IActionResult> Index(int theatreId)
        {
            if (theatreId <= 0)
            {
                TempData["Error"] = "Invalid Theatre ID.";
                return RedirectToAction("Index", "Theatre");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_appBaseUrl}ScreenAPI/theatre/{theatreId}");

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize into ScreenDTO, not Screen
                    var screens = await response.Content.ReadFromJsonAsync<List<ScreenDTO>>() ?? new List<ScreenDTO>();
                    Relation.TheatreId = theatreId;
                    return View(screens);
                }

                TempData["Error"] = "Failed to load screens.";
                return RedirectToAction("Index", "Theatre", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching screens: " + ex.Message);
                TempData["Error"] = "Error fetching screens.";
                return RedirectToAction("Index", "Theatre", new { area = "Admin" });
            }
        }
        #endregion

        #region GET ADD OR EDIT SCREEN
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            if (id == null || id == 0)
            {
                // New screen
                var newScreen = new ScreenAddDTO { TheatreId = Relation.TheatreId };
                return View(newScreen);
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_appBaseUrl}ScreenAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var screen = await response.Content.ReadFromJsonAsync<ScreenAddDTO>() ?? new ScreenAddDTO { TheatreId = Relation.TheatreId };
                    return View(screen);
                }

                TempData["Error"] = "Screen not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching screen: " + ex.Message);
                TempData["Error"] = "Failed to load screen.";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region POST ADD OR EDIT SCREEN
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ScreenAddDTO model)
        {

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Enter Valid Data!.";
                return View(model);
            }

            HttpResponseMessage response;
            if (model.ScreenNo == 0 || model.ScreenId == null) 
            {
                response = await _httpClient.PostAsJsonAsync($"{_appBaseUrl}ScreenAPI", model);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"{_appBaseUrl}ScreenAPI", model);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = model.ScreenId == 0 || model.ScreenId == null ? "Screen added successfully!" : "Screen updated successfully!";
                return RedirectToAction("Index", new { theatreId = Relation.TheatreId });
            }

            Console.WriteLine("Error saving screen: " + response.ReasonPhrase);
            TempData["Error"] = "Failed to save screen.";
            return View(model);
        }
        #endregion

        #region DELETE SCREEN
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ids),
                                                Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_appBaseUrl}ScreenAPI/DeleteScreens", content);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TempData["Success"] = "All selected screens deleted successfully!";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                    {
                        TempData["Error"] = "Some screens deleted, some not found.";
                    }
                }
                else
                {
                    TempData["Error"] = "No screens could be deleted.";
                }

                return RedirectToAction("Index", new { theatreId = Relation.TheatreId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting screens: " + ex.Message);
                TempData["Error"] = "Failed to delete screens.";
                return RedirectToAction("Index", new { theatreId = Relation.TheatreId });
            }
        }
        #endregion

    }
}
