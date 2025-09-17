using System.ComponentModel.DataAnnotations;
using Movie_management_system.Models;

namespace Movie_management_system.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int BookingCount { get; set; }
    }
    public class UserAddDTO
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        public Boolean? IsActive { get; set; } = true;
    }
    public class UserEditDTO
    {


        [Required]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
