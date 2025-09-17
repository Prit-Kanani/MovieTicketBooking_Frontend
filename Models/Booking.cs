using System;
using System.Collections.Generic;

namespace Movie_management_system.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int? ShowId { get; set; }
        public string BookingType { get; set; }
        public DateTime? DateTime { get; set; }
        public string PaymentStatus { get; set; }

        public bool IsActive { get; set; } = true;

        public List<SeatsBooked>? SeatsBookeds { get; set; }
        public ShowTime? Show { get; set; }
        public User? User { get; set; }
    }
}
