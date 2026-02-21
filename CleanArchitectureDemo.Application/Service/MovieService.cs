using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Service
{
    public class MovieService(IMovieRepository movieRepository) : IMovieService
    {
        public async Task<IReadOnlyList<Movie>> GetAllMoviesAsync(
            CancellationToken cancellationToken = default)
        {
            return await movieRepository
                .GetAllMoviesAsync(cancellationToken);
        }
    }
}