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
    public class TheatreController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public TheatreController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region GET THEATRE
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Theatre> theatres = new();

            try
            {
                var theatresResponse = await _httpClient.GetAsync(_appBaseUrl + "TheatreAPI");
                if (theatresResponse.IsSuccessStatusCode)
                    theatres = await theatresResponse.Content.ReadFromJsonAsync<List<Theatre>>() ?? new List<Theatre>();
                foreach (var theatre in theatres)
                {
                    theatre.Screens ??= new List<Screen>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching Theatres data: " + ex.Message);
                ViewBag.Error = "Failed to load Theatres.";
            }

            ViewBag.Theatres = theatres;
            return View();
        }
        #endregion

        #region GET ADD OR EDIT THEATRE
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int? theatreId)
        {

            if (theatreId == null || theatreId == 0)
            {
                return View(new TheatreDTO());
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_appBaseUrl}TheatreAPI/{theatreId}");
                if (response.IsSuccessStatusCode)
                {
                    var theatre = await response.Content.ReadFromJsonAsync<TheatreDTO>() ?? new TheatreDTO();
                    return View(theatre);
                }

                TempData["Error"] = "Theatre not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching theatre: " + ex.Message);
                TempData["Error"] = "Failed to load theatre.";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region POST ADD OR EDIT THEATRE
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(TheatreDTO model)
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix form errors.";
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            var dto = new TheatreDTO
            {
                TheatreId = model.TheatreId == 0 ? null : model.TheatreId,
                Name = model.Name,
                City = model.City,
                Address = model.Address,
                UserId = TokenManager.UserId,
            };

            HttpResponseMessage response;
            if (dto.TheatreId == null)
            {
                response = await _httpClient.PostAsJsonAsync($"{_appBaseUrl}TheatreAPI", dto);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"{_appBaseUrl}TheatreAPI/{dto.TheatreId}", dto);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = dto.TheatreId == null || dto.TheatreId == 0 ? "Theatre added successfully!" : "Theatre updated successfully!";
                return RedirectToAction("Index");
            }
            Console.WriteLine("Error saving theatre: " + response.ReasonPhrase);
            TempData["Error"] = "Failed to save theatre.";
            return View(model);
        }
        #endregion

        #region DELETE THEATRE
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ids),
                                                Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_appBaseUrl}TheatreAPI/DeleteTheatres", content);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TempData["Success"] = "All selected theatres deleted successfully!";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                    {
                        TempData["Error"] = "Some theatres deleted, some not found.";
                    }
                }
                else
                {
                    TempData["Error"] = "No theatres could be deleted.";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting theatres: " + ex.Message);
                TempData["Error"] = "Failed to delete theatres.";
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}
