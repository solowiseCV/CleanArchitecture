using MediatR;

namespace CleanArchitecture.Application.Movies.Commands;

public record DeleteMovieCommand(Guid Id) : IRequest<Unit>;
