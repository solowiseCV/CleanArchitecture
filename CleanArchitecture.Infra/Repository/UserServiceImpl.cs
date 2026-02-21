using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class UserServiceImpl : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UserServiceImpl(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                throw new BadRequestException($"Registration failed: {string.Join(", ", errors)}");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var accessToken = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

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

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return _mapper.Map<CurrentUserResponse>(user);
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

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
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null)
            {
                throw new BadRequestException("Invalid refresh token.");
            }

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
    }
}
