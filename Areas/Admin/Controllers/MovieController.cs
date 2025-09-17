using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;
using Movie_management_system.Models;
using Newtonsoft.Json;

namespace Movie_management_system.Areas.Admin.Controllers
{
    [Area("Admin")]
    [JwtAuthFilter]
    public class MovieController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public MovieController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion  

        #region GET ALL MOVIES
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            List<MovieDTO> movies = new();

            try
            {
                var movieResponse = await _httpClient.GetAsync(_appBaseUrl + "MovieAPI");
                if (movieResponse.IsSuccessStatusCode)
                    movies = await movieResponse.Content.ReadFromJsonAsync<List<MovieDTO>>() ?? new List<MovieDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movie data: " + ex.Message);
                TempData["Error"] = "Failed to load movies.";
            }

            ViewBag.Movies = movies;
            return View();
        }
        #endregion

        #region GET ADD OR EDIT MOVIE
        [HttpGet]
        public async Task<IActionResult> AddMovie(int? id)
        {
            if (string.IsNullOrEmpty(TokenManager.Token))
                return RedirectToAction("Index", "Login", new { area = "" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            var genres = await LoadGenresAsync(); // gets all genres with ID + Name
            ViewBag.Genres = genres;

            if (id == null || id == 0)
                return View(new Movie());

            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"MovieAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var movie = await response.Content.ReadFromJsonAsync<Movie>() ?? new Movie();

                    movie.GenreIds = genres
                        .Where(g => movie.Genres.Select(mg => mg.Name).Contains(g.Name))
                        .Select(g => g.GenreId)
                        .ToList();

                    return View(movie);
                }
                else
                {
                    TempData["Error"] = "Movie not found.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching movie: " + ex.Message);
                TempData["Error"] = "Failed to load movie.";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region POST ADD OR EDIT MOVIE
        [HttpPost]
        public async Task<IActionResult> AddMovie(Movie model)
        {
            ModelState.Remove("Poster");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                ViewBag.Genres = await LoadGenresAsync();
                return View(model);
            }

            string posterFileName = model.Poster; // fallback to old if no new file
            if (model.PosterFile != null && model.PosterFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);

                string originalFileName = Path.GetFileName(model.PosterFile.FileName);
                string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PosterFile.CopyToAsync(stream);
                }

                posterFileName = uniqueFileName;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            var dto = new AddMovieDTO
            {
                MovieId = model.MovieId, // include ID to detect edit
                Name = model.Name,
                Language = model.Language,
                Duration = model.Duration,
                Description = model.Description,
                Poster = posterFileName,
                GenreIds = model.GenreIds
            };

            HttpResponseMessage response;
            if (model.MovieId == null || model.MovieId == 0)
            {
                // Add new movie
                response = await _httpClient.PostAsJsonAsync(_appBaseUrl + "MovieAPI", dto);
            }
            else
            {
                // Edit existing movie
                response = await _httpClient.PutAsJsonAsync($"{_appBaseUrl}MovieAPI/{model.MovieId}", dto);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = model.MovieId == 0 ? "Movie added successfully!" : "Movie updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to save movie.";
            ViewBag.Genres = await LoadGenresAsync();
            return View(model);
        }
        #endregion

        #region LOAD GENRES
        private async Task<List<Genre>> LoadGenresAsync()
        {
            var response = await _httpClient.GetAsync("Genres");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var genres = JsonConvert.DeserializeObject<List<Genre>>(json);
                return genres ?? new List<Genre>();
            }
            return new List<Genre>();
        }
        #endregion

        #region DELETE MOVIE
        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);

            // Step 1: Get movie to fetch poster filename
            string posterFileName = null;
            var getResponse = await _httpClient.GetAsync($"{_appBaseUrl}MovieAPI/{id}");
            if (getResponse.IsSuccessStatusCode)
            {
                var movieJson = await getResponse.Content.ReadAsStringAsync();
                var movie = JsonConvert.DeserializeObject<DeleteMovieDTO>(movieJson);
                posterFileName = movie?.Poster;
            }

            // Step 2: Delete movie
            var deleteResponse = await _httpClient.DeleteAsync($"{_appBaseUrl}MovieAPI/{id}");

            if (deleteResponse.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(posterFileName))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", posterFileName);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                TempData["Success"] = "Movie deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the movie.";
            }

            return RedirectToAction("Index");
        }
        #endregion

    }
}