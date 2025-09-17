namespace Movie_management_system.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public int ScreenNo { get; set; }

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

        public virtual Theatre Theatre { get; set; } = null!;
    }
}
