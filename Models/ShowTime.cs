﻿namespace Movie_management_system.Models
{
    public partial class ShowTime
    {
        public int ShowId { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual Movie Movie { get; set; } = null!;

        public virtual Screen Screen { get; set; } = null!;
    }
}
