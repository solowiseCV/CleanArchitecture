using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Movies.Queries;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class GetAllMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, IUserServices userServices) 
    : IRequestHandler<GetAllMoviesQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(GetAllMoviesQuery query, CancellationToken cancellationToken)
    {
        var movies = await unitOfWork.Movies.GetAllMoviesAsync(cancellationToken);
        var userId = currentUserService.GetUserId();
        bool isPremiumUser = false;

        if (!string.IsNullOrEmpty(userId))
        {
            isPremiumUser = await userServices.IsPremiumAsync(userId);
        }

        var responses = mapper.Map<List<MovieResponse>>(movies.ToList());

        foreach (var response in responses)
        {
            response.IsAccessible = !response.IsPremium || isPremiumUser;
        }

        return responses;
    }
}
