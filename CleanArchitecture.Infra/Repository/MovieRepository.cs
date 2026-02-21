using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{

    public class MovieRepository(ApplicationDbContext context) : IMovieRepository
    {

        public async Task<IReadOnlyList<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken = default)
        {
            return await context.Movies
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Movies.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            await context.Movies.AddAsync(movie, cancellationToken);
        }

        public void Update(Movie movie)
        {
            context.Movies.Update(movie);
        }

        public void Delete(Movie movie)
        {
            context.Movies.Remove(movie);
        }
    }
}