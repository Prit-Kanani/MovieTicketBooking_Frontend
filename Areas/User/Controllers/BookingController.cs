using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Movie_management_system.DTOs;
using Movie_management_system.Helper;
using static System.Net.WebRequestMethods;
namespace Movie_management_system.Areas.User.Controllers
{
    [Area("User")]
    [JwtAuthFilter]
    public class BookingController : Controller
    {
        #region CONFIGURATION
        private readonly HttpClient _httpClient;
        private readonly string _appBaseUrl;

        // ✅ Inject IConfiguration here
        public BookingController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IWebHostEnvironment env, ILogger<DashboardController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _appBaseUrl = configuration["AppBaseUrl"];
        }
        #endregion

        #region INDEX
        public async Task<IActionResult> Index(int? id)
        {
            var userId = TokenManager.UserId;
            var resp = await _httpClient.GetAsync($"{_appBaseUrl}ShowTimeAPI/{id}/seatmap?userId={userId}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var vm = await resp.Content.ReadFromJsonAsync<ShowSeatMapDTO>();
            if (vm == null) return NotFound();

            ViewBag.UserId = userId;
            return View(vm);
        }
        #endregion

        #region CREATE - BOOKING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(int showId, List<int> selectedSeats)
        {
            var userId = TokenManager.UserId;
            if (userId == 0) return Unauthorized();

            // 🚨 New validation
            if (selectedSeats == null || !selectedSeats.Any())
            {
                TempData["Error"] = "Please select at least one seat before booking.";
                return RedirectToAction("Index", new { id = showId });
            }

            var dto = new CreateBookingDTO
            {
                UserId = userId,
                ShowId = showId,
                SeatNos = selectedSeats ?? new List<int>(),
                PaymentStatus = "Completed" // or "Pending" depending on flow
            };
            Console.WriteLine($"Booking DTO: {System.Text.Json.JsonSerializer.Serialize(dto)}");

            var resp = await _httpClient.PostAsJsonAsync($"{_appBaseUrl}BookingAPI/booking", dto);
            if (resp.IsSuccessStatusCode)
            {
                var ok = await resp.Content.ReadFromJsonAsync<BookingResultDTO>();
                TempData["Success"] = $"Booking confirmed!";
                return RedirectToAction("Index", new { id = showId });
            }

            if ((int)resp.StatusCode == 409)
            {
                var conflict = await resp.Content.ReadFromJsonAsync<BookingResultDTO>();
                TempData["Error"] = $"Seat(s) already booked: {string.Join(", ", conflict?.Conflicts ?? new())}. Please reselect.";
                return RedirectToAction("Index", new { id = showId });
            }

            TempData["Error"] = "Booking failed. Try again.";
            return RedirectToAction("Index", new { id = showId });
        }
        #endregion

        #region GENERATE RANDOM TICKET NO.
        private string GenerateTicketNumber()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        #endregion

        public IActionResult TestPdf()
        {
            var model = new { Name = "hello" };
            return new ViewAsPdf("TestPdfView", model) { FileName = "test.pdf" };
        }
    }
}
