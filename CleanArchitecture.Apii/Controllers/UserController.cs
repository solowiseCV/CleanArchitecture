using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController(IUserServices userServices) : ControllerBase
    {
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register a new user", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponse>> Register(UserRegisterRequest request)
        {
            var result = await userServices.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login user and return token", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> Login(UserLoginRequest request)
        {
            var result = await userServices.LoginAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("current")]
        [SwaggerOperation(Summary = "Get current authenticated user", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CurrentUserResponse>> GetCurrent()
        {
            var result = await userServices.GetCurrentUserAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user by id", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetById(Guid id)
        {
            var result = await userServices.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update user", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request)
        {
            var result = await userServices.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete user", Tags = new[] { "Users" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await userServices.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("refresh-token")]
        [SwaggerOperation(Summary = "Refresh authentication token", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CurrentUserResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await userServices.RefreshTokenAsync(request);
            return Ok(result);
        }

        [HttpPost("revoke-token")]
        [SwaggerOperation(Summary = "Revoke a refresh token", Tags = new[] { "Users" })]
        [ProducesResponseType(typeof(RevokeRefreshTokenResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RevokeRefreshTokenResponse>> RevokeToken(RefreshTokenRequest request)
        {
            var result = await userServices.RevokeRefreshToken(request);
            return Ok(result);
        }
    }
}
