using System.ComponentModel.DataAnnotations;

namespace Movie_management_system.DTOs
{
    public class MovieDTO
    {
        public int? MovieId { get; set; }

        [Required(ErrorMessage = "Movie name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Language is required.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int Duration { get; set; }


        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        // Poster is expected to be a file path or base64 string; not validating here unless necessary
        public string Poster { get; set; }

        [Required(ErrorMessage = "At least one genre is required.")]
        public virtual List<string> Genres { get; set; }
    }
    public class AddMovieDTO
    {
        public int? MovieId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public List<int> GenreIds { get; set; }
    }
    public class DeleteMovieDTO
    {
        public int? MovieId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public List<int> GenreIds { get; set; }
    }
}
