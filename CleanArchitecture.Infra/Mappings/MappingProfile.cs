using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Infrastructure.Identity;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            
            CreateMap<ApplicationUser, CurrentUserResponse>();
            
            CreateMap<UserRegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateUserRequest, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Movie Mappings (Assuming Movie exists in Domain)
            // CreateMap<Movie, MovieResponse>();
        }
    }
}
