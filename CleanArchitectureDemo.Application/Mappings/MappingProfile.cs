using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUserDto, UserResponse>();
            CreateMap<ApplicationUserDto, CurrentUserResponse>();

            // Movie Mappings
            CreateMap<Movie, MovieResponse>();
            CreateMap<CreateMovieRequest, Movie>();
            CreateMap<UpdateMovieRequest, Movie>();
            CreateMap<UserRegisterRequest, ApplicationUserDto>();

        }
    }
}
