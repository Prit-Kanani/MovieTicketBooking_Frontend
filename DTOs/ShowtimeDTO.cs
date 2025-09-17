namespace Movie_management_system.DTOs
{
    public class ShowtimeDTO
    {
        public int ShowId { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int BookingsCount { get; set; }

        public string MovieName { get; set; } = null!;
    }
    public class ShowtimeAddDTO
    {
        public int? ShowId { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public decimal Price { get; set; }
    }
    public class ShowSeatMapDTO
    {
        public int ShowId { get; set; }
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public string Theatre { get; set; } = "";
        public int ScreenNo { get; set; }

        public List<int> MyBookedSeats { get; set; } = new();
        public List<int> OthersBookedSeats { get; set; } = new();
    }

    public class CreateBookingDTO
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public List<int> SeatNos { get; set; } = new();
        public string PaymentStatus { get; set; } = "Pending"; // or "Paid" from your UI flow
    }

    public class BookingResultDTO
    {
        public int BookingId { get; set; }
        public List<int> ConfirmedSeats { get; set; } = new();
        public List<int> Conflicts { get; set; } = new();
        public string Message { get; set; } = "";
    }
}
