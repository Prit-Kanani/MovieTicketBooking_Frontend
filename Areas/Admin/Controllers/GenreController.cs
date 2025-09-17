using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.Models;
using Movie_management_system.Helper;
using Newtonsoft.Json;

namespace Movie_management_system.Areas.Admin.Controllers
{
    [Area("Admin")]
    [JwtAuthFilter]
    public class GenreController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public GenreController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region GET ALL GENRES
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            List<Genre> genres = new();
            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + "Genres");
                if (response.IsSuccessStatusCode)
                    genres = await response.Content.ReadFromJsonAsync<List<Genre>>() ?? new List<Genre>();
                Console.WriteLine("response == "+response);
                Console.WriteLine("Genres fetched successfully." + genres);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching genres: " + ex.Message);
                TempData["Error"] = "Failed to load genres.";
            }

            ViewBag.Genres = genres;
            return View();
        }
        #endregion

        #region ADD OR EDIT GENRE
        [HttpGet]
        public async Task<IActionResult> AddOrEditGenre(int? id)
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            // If no ID -> it's Add mode
            if (!id.HasValue)
                return View(new Genre());

            // If ID exists -> it's Edit mode
            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"Genres/{id.Value}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("response == " + response);
                    var genre = await response.Content.ReadFromJsonAsync<Genre>();
                    return View(genre);
                }
                TempData["Error"] = "Genre not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching genre: " + ex.Message);
                TempData["Error"] = "An error occurred while fetching the genre.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditGenre(int? id, Genre genre)
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response;

                    if (id.HasValue && id > 0) // Edit mode
                        response = await _httpClient.PutAsJsonAsync(_appBaseUrl + $"Genres/{id.Value}", genre);
                    else // Add mode
                        response = await _httpClient.PostAsJsonAsync(_appBaseUrl + "Genres", genre);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = id.HasValue ? "Genre updated successfully!" : "Genre added successfully!";
                        return RedirectToAction("Index");
                    }

                    TempData["Error"] = id.HasValue ? "Failed to update genre." : "Failed to add genre.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving genre: " + ex.Message);
                    TempData["Error"] = "An error occurred while saving the genre.";
                }
            }

            return View(genre);
        }
        #endregion

        #region DELETE GENRE
        [HttpGet]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            try
            {
                var response = await _httpClient.DeleteAsync(_appBaseUrl + $"Genres/{id}");
                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Genre deleted successfully.";
                else
                    TempData["Error"] = "Failed to delete genre.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting genre: " + ex.Message);
                TempData["Error"] = "An error occurred while deleting the genre.";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region GENRES PAGINATION
        public async Task<IActionResult> Pagination(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"Genres/paginate?pageNumber={page}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PaginatedGenresResponse>();
                    ViewBag.Genres = result?.Data ?? new List<Genre>();
                    ViewBag.CurrentPage = result?.CurrentPage ?? 1;
                    ViewBag.TotalPages = result?.TotalPages ?? 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching genres: " + ex.Message);
                TempData["Error"] = "Failed to load genres.";
                ViewBag.Genres = new List<Genre>();
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 1;
            }

            return View("Index");
        }
        #endregion

        public class PaginatedGenresResponse
        {
            public List<Genre> Data { get; set; } = new();
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }

    }
}
