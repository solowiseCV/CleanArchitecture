using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs
{
    public class UpdateUserRequest
    {
    public  string? FirstName { get; set; }
    public  string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? Gender { get; set; }
}


public class RevokeRefreshTokenResponse
{
    public required string Message { get; set; }
}


public class RefreshTokenRequest
{
    public required string RefreshToken { get; set; }
}

}
