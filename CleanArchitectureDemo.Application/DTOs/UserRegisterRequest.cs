using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
    public class UserRegisterRequest
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required  string LastName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8)]
        public required string Password { get; set; }
        [Required]
        public  required string Gender { get; set; }
    }
}
