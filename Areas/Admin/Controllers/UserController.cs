using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;
using Newtonsoft.Json.Linq;
using Movie_management_system.Models;
using Newtonsoft.Json;

namespace Movie_management_system.Areas.Admin.Controllers
{
    [Area("Admin")]
    [JwtAuthFilter]
    public class UserController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        public UserController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region GET ALL USERS
        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = new();

            try
            {
                var userResponse = await _httpClient.GetAsync(_appBaseUrl + "UserAPI");
                if (userResponse.IsSuccessStatusCode)
                    users = await userResponse.Content.ReadFromJsonAsync<List<UserDTO>>() ?? new List<UserDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching Users data: " + ex.Message);
                TempData["Error"] = "Failed to load Users.";
            }

            ViewBag.Users = users;
            return View();
        }
        #endregion

        #region ADD USER POST

        #region ADD USER
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AddUser(UserAddDTO user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(_appBaseUrl + "UserAPI", user);
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<APIResponse>(json);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = result?.Message;
                        return RedirectToAction("Index"); 
                    }
                    else
                    {
                        TempData["Error"] = result?.Message;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error adding User: " + ex.Message);
                    TempData["Error"] = "An error occurred while adding the User.";
                }
            }
            return View(user);
        }
        #endregion

        #region DELETE USER
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(_appBaseUrl + "UserAPI/" + id);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete User.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting User: " + ex.Message);
                TempData["Error"] = "An error occurred while deleting the User.";
            }
            return RedirectToAction("Index");
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
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
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
