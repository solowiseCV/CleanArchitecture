using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class UserServiceImpl(
        UserManager<ApplicationUser> _userManager, 
        SignInManager<ApplicationUser> _signInManager, 
        ITokenService _tokenService, 
        ICurrentUserService _currentUserService, 
        IMapper _mapper,
        ILogger<UserServiceImpl> _logger) : IUserServices
    {
        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registering new user: {Email}", request.Email);
            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", request.Email, string.Join(", ", errors));
                throw new BadRequestException($"Registration failed: {string.Join(", ", errors)}");
            }

            _logger.LogInformation("User registered successfully: {Email}", request.Email);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found: {Email}", request.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }
         
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed. Invalid password for user: {Email}", request.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }

            var accessToken = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            var response = _mapper.Map<UserResponse>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;

            return response;
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException("User not found or not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");
          
            return _mapper.Map<CurrentUserResponse>(user);
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User not found."); ;
        
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User not found."); ;
          
            _mapper.Map(request, user);
            user.UpdateAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Update failed.");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken) ?? throw new BadRequestException("Invalid refresh token."); 
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return new RevokeRefreshTokenResponse { Message = "Token revoked." };
        }

        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            var newAccessToken = await _tokenService.CreateToken(user);
            
            return new CurrentUserResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Gender = user.Gender,
                AccessToken = newAccessToken,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt
            };
        }

        public async Task<bool> IsPremiumAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.IsPremiumUser ?? false;
        }

        public async Task UpdatePremiumStatusAsync(string userId, bool isPremium)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");
            user.IsPremiumUser = isPremium;
            user.PremiumExpiry = isPremium ? DateTime.UtcNow.AddMonths(1) : null;
            await _userManager.UpdateAsync(user);
        }
    }
}
