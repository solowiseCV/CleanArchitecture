using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Movies.Commands;

public record UpdateMovieCommand(Guid Id, UpdateMovieRequest Request) : IRequest<Unit>;
