namespace CleanArchitecture.Application.DTOs
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public  required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }


    }
}
