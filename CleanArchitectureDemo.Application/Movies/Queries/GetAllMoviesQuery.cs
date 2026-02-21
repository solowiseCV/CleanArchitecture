using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Movies.Queries;

public record GetAllMoviesQuery : IRequest<List<MovieResponse>>;
