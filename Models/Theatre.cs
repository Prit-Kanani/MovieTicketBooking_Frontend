namespace Movie_management_system.Models
{
    public class Theatre
    {
        public int TheatreId { get; set; }

        public string Name { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Address { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();

        public virtual User? User { get; set; } = null;
    }
}
