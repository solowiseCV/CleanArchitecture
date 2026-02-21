using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.IService
{
    public interface ITokenService
    {
        Task<string> CreateToken(IdentityUser user);
        string CreateRefreshToken();
    }
}
