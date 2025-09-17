namespace Movie_management_system.DTOs
{
    public class BookingDTO
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public string BookingType { get; set; }
        public string PaymentStatus { get; set; }
        public List<string> SeatNos { get; set; }
    }
    public class BookingResponseDTO
    {
        public int BookingId { get; set; }
        public string BookingType { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? DateTime { get; set; }
        public List<string> SeatNos { get; set; }
        public string? MovieName { get; set; }
        public string? UserName { get; set; }
    }
}
