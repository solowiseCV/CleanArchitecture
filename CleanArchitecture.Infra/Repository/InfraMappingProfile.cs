using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Infrastructure.Identity;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class InfraMappingProfile : Profile
    {
        public InfraMappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
        }
    }
}
