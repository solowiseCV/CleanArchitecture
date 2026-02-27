using CleanArchitecture.Application.IRepository;

namespace CleanArchitecture.Application.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        IPaymentRepository Payments { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
