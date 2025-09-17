namespace Movie_management_system.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual ICollection<Theatre> Theatres { get; set; } = new List<Theatre>();
    }
}
