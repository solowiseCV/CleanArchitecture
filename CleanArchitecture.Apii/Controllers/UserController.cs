using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserServices userServices) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(UserRegisterRequest request)
        {
            var result = await userServices.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login(UserLoginRequest request)
        {
            var result = await userServices.LoginAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<CurrentUserResponse>> GetCurrent()
        {
            var result = await userServices.GetCurrentUserAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(Guid id)
        {
            var result = await userServices.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request)
        {
            var result = await userServices.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await userServices.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<CurrentUserResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await userServices.RefreshTokenAsync(request);
            return Ok(result);
        }

        [HttpPost("revoke-token")]
        public async Task<ActionResult<RevokeRefreshTokenResponse>> RevokeToken(RefreshTokenRequest request)
        {
            var result = await userServices.RevokeRefreshToken(request);
            return Ok(result);
        }
    }
}
