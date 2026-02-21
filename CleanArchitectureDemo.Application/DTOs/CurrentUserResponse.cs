using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }

    public class CurrentUserResponse
    {

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public  string? AccessToken { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

    }

}
