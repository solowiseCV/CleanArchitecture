using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.Movies.Queries;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class GetMovieByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    public async Task<MovieResponse> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
    {
        var movie = await unitOfWork.Movies.GetByIdAsync(query.Id, cancellationToken);
        
        if (movie == null)
        {
            throw new NotFoundException(nameof(Movie), query.Id);
        }

        return mapper.Map<MovieResponse>(movie);
    }
}
