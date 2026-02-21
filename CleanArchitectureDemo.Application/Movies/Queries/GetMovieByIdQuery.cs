using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Movies.Queries;

public record GetMovieByIdQuery(Guid Id) : IRequest<MovieResponse>;
