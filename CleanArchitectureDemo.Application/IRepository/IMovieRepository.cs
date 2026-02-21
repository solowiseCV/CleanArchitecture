using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.IRepository
{
    public interface IMovieRepository
    {
        Task<IReadOnlyList<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken = default);
        Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Movie movie, CancellationToken cancellationToken = default);
        void Update(Movie movie);
        void Delete(Movie movie);
    }
}