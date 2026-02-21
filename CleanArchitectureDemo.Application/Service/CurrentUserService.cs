using System.Security.Claims;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Service;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? GetUserId()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
