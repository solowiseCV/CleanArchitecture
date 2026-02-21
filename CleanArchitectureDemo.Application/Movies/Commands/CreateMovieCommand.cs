using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Movies.Commands;

public record CreateMovieCommand(CreateMovieRequest Request) : IRequest<MovieResponse>;
