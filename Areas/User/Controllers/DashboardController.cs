using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;


namespace Movie_management_system.Areas.User.Controllers
{
    [Area("User")]
    [JwtAuthFilter]
    public class DashboardController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DashboardController> _logger;

        // ✅ Inject IConfiguration here
        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IWebHostEnvironment env, ILogger<DashboardController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
            _env = env;
            _logger = logger;
        }
        #endregion

        #region DASHBOARD INDEX
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
            ViewBag.TopMovies = movies?.Take(10).ToList() ?? new List<MovieDTO>();

            return View();
        }
        #endregion

        #region LOGOUT
        public IActionResult Logout()
        {
            // Clear token + related session state
            TokenManager.Token = null;
            TokenManager.UserId = 0;
            TokenManager.Email = null;

            Console.WriteLine("User logged out, token cleared.");
            return RedirectToAction("Index", "Login", new { area = "" });
        }
        #endregion

        #region DETAILS (GET)
        public async Task<IActionResult> Details(int id)
        {
            UserEditDTO user = null;

            try
            {
                var response = await _httpClient.GetAsync(_appBaseUrl + $"UserAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    user = await response.Content.ReadFromJsonAsync<UserEditDTO>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    ViewBag.Error = $"API returned {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Details(GET) error for user id {Id}", id);
                ViewBag.Error = "Failed to load user details.";
            }

            if (user == null)
                return NotFound();

            // Determine profile image URL (search images folder for sanitizedEmail.*)
            try
            {
                var safeName = SanitizeFileName(user.Email);
                var imagesDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                if (Directory.Exists(imagesDir))
                {
                    var found = Directory.GetFiles(imagesDir, safeName + ".*").FirstOrDefault();
                    if (found != null)
                    {
                        var fileName = Path.GetFileName(found);
                        ViewBag.ProfileImageUrl = Url.Content($"~/images/{fileName}") + $"?t={DateTime.UtcNow.Ticks}";
                    }
                    else
                    {
                        ViewBag.ProfileImageUrl = Url.Content("~/images/default_user.png");
                    }
                }
                else
                {
                    ViewBag.ProfileImageUrl = Url.Content("~/images/default_user.png");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not resolve profile image for user {Email}", user.Email);
                ViewBag.ProfileImageUrl = Url.Content("~/images/default_user.png");
            }

            return View(user);
        }
        #endregion

        #region DETAILS (PATCH) - update user + optional image save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(UserEditDTO model, IFormFile? ProfileImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the highlighted fields.";
                return View(model);
            }

            try
            {
                // 1) Save/replace profile image in wwwroot/images/{sanitizedEmail}{ext}
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    try
                    {
                        var savedFileName = await SaveProfileImageAsync(ProfileImage, model.Email);
                        if (string.IsNullOrEmpty(savedFileName))
                        {
                            _logger.LogWarning("SaveProfileImageAsync returned null/empty for {Email}", model.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving profile image for {Email}", model.Email);
                        TempData["Error"] = "Failed to save profile image.";
                        return View(model);
                    }
                }

                // 2) Call API to update user (no image info included)
                var putResponse = await _httpClient.PatchAsJsonAsync(_appBaseUrl + $"UserAPI", model);
                if (putResponse.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Profile updated successfully.";
                    TokenManager.Name = model.Name; // update name in session
                    return RedirectToAction("Index","Dashboard",new {Areas="User"});
                }
                else
                {
                    var body = await putResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning("User update failed for {Id}: {Status} - {Body}", model.UserId, putResponse.StatusCode, body);
                    TempData["Error"] = $"Failed to update profile. API returned {putResponse.StatusCode}";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Details(POST) for user {Id}", model.UserId);
                TempData["Error"] = "An unexpected error occurred.";
                return View(model);
            }
        }
        #endregion

        #region HELPERS : file handling & sanitization
        private static string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "unknown";

            // replace invalid filename chars with underscore
            var invalid = Path.GetInvalidFileNameChars();
            var sanitized = new string(input.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());

            // optional: limit length
            if (sanitized.Length > 150) sanitized = sanitized.Substring(0, 150);

            return sanitized;
        }

        private async Task<string?> SaveProfileImageAsync(IFormFile file, string userEmail)
        {
            if (file == null || file.Length == 0) return null;

            var imagesDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(imagesDir))
                Directory.CreateDirectory(imagesDir);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".png";

            var safeName = SanitizeFileName(userEmail ?? "user");

            // Delete any existing files for this user (any extension)
            var existing = Directory.GetFiles(imagesDir, safeName + ".*");
            foreach (var p in existing)
            {
                try { System.IO.File.Delete(p); }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete existing image {Path}", p);
                }
            }

            var newFileName = $"{safeName}{ext}";
            var newPath = Path.Combine(imagesDir, newFileName);

            // write file (overwrites if exists)
            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return newFileName;
        }
        #endregion
    }
}
