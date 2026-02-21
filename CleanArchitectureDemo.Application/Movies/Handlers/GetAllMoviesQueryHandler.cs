using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.Movies.Queries;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class GetAllMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllMoviesQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(GetAllMoviesQuery query, CancellationToken cancellationToken)
    {
        var movies = await unitOfWork.Movies.GetAllMoviesAsync(cancellationToken);
        return mapper.Map<List<MovieResponse>>(movies.ToList());
    }
}
