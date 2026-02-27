using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Infrastructure.Context;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class UnitOfWork(ApplicationDbContext context, IMovieRepository movies, IPaymentRepository payments) : IUnitOfWork
    {
        public IMovieRepository Movies { get; } = movies;
        public IPaymentRepository Payments { get; } = payments;

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
