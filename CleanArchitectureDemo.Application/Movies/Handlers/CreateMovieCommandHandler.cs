using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.Movies.Commands;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class CreateMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<CreateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = mapper.Map<Movie>(command.Request);
        
        await unitOfWork.Movies.AddAsync(movie, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return mapper.Map<MovieResponse>(movie);
    }
}
