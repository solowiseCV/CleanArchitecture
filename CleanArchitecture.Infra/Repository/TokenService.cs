using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class TokenService(IConfiguration _configuration, UserManager<ApplicationUser> _userManager) : ITokenService
    {
       

        public async Task<string> CreateToken(IdentityUser user)
        {
            var applicationUser = user as ApplicationUser;
            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var userRoles = await _userManager.GetRolesAsync(applicationUser!);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id),
            };

            if (applicationUser != null)
            {
                authClaims.Add(new Claim("firstName", applicationUser.FirstName));
                authClaims.Add(new Claim("lastName", applicationUser.LastName));
            }

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key!));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.ValidIssuer,
                audience: jwtSettings.ValidAudience,
                expires: DateTime.Now.AddMinutes(jwtSettings.Expires),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
