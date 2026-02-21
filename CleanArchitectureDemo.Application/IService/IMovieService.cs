using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.IService
{
    public interface IMovieService
    {
        Task<IReadOnlyList<Movie>> GetAllMoviesAsync(
            CancellationToken cancellationToken = default);
    }
}