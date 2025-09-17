using Movie_management_system.Models;

namespace Movie_management_system.DTOs
{
    public class ScreenDTO
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public int ScreenNo { get; set; }

        public int TotalSeats { get; set; }

        public  int? ShowTimes { get; set; }

        public virtual Theatre Theatre { get; set; } = null!;
    }
    public class ScreenAddDTO
    {
        public int? ScreenId { get; set; }
        public int TheatreId { get; set; }
        public int ScreenNo { get; set; }
        public int TotalSeats { get; set; }
    }
}
