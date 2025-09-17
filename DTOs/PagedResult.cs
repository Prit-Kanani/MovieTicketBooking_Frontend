namespace Movie_management_system.DTOs
{
    public class MovieDetailsViewDTO
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public List<string> Genres { get; set; }
        public List<MovieTheatreViewDTO> Theatres { get; set; }
    }

    public class MovieTheatreViewDTO
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public List<MovieScreenViewDTO> Screens { get; set; }
    }

    public class MovieScreenViewDTO
    {
        public int ScreenId { get; set; }
        public int ScreenNo { get; set; }
        public int TotalSeats { get; set; }
        public List<MovieShowTimeViewDTO> ShowTimes { get; set; }
    }

    public class MovieShowTimeViewDTO
    {
        public int ShowId { get; set; }
        public string Date { get; set; }   // ✅ safer for frontend display
        public string Time { get; set; }   // ✅ safer for frontend display
        public decimal Price { get; set; }
    }
}
