using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movie_management_system.Models
{
    public class Movie
    {
        public int? MovieId { get; set; }

        [Required(ErrorMessage = "Movie name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Language is required.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int Duration { get; set; }

        public string Poster { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Poster file is required.")]
        public IFormFile PosterFile { get; set; }

        [Required(ErrorMessage = "At least one genre is required.")]
        public List<int> GenreIds { get; set; } = new();
        public List<Genre> Genres { get; set; } = new List<Genre>();

        public bool IsActive { get; set; } = true;

    }

}
