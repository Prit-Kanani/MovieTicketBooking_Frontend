using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;

namespace Movie_management_system.Areas.User.Controllers
{
    [Area("User")]
    [JwtAuthFilter]
    public class MovieController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        // ✅ Inject IConfiguration here
        public MovieController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IWebHostEnvironment env, ILogger<DashboardController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region MOVIE INDEX
        public async Task<IActionResult> Index()
        {
            List<MovieDTO> movies = new();

            try
            {
                var movieResponse = await _httpClient.GetAsync(_appBaseUrl + "MovieAPI");
                if (movieResponse.IsSuccessStatusCode)
                    movies = await movieResponse.Content.ReadFromJsonAsync<List<MovieDTO>>() ?? new List<MovieDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to load dashboard data.";
            }
            ViewBag.Movies = movies?.Take(10).ToList() ?? new List<MovieDTO>();

            return View();
        }
        #endregion

        #region MOVIE DETAILS
            public async Task<IActionResult> Details(int id)
            {
                var movie = new MovieDetailsViewDTO();

                try
                {
                    var response = await _httpClient.GetAsync($"{_appBaseUrl}MovieAPI/{id}/fulldetails");
                    if (response.IsSuccessStatusCode)
                    {
                        movie = await response.Content.ReadFromJsonAsync<MovieDetailsViewDTO>();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Failed to load movie details.";
                }

                if (movie == null)
                    return NotFound();

                return View(movie);
            }
            #endregion

    }
}
