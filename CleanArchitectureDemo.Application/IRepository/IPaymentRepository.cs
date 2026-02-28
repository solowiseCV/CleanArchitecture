using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.IRepository
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
        void Update(Payment payment);
        Task<bool> TryUpdateStatusAsync(string reference, PaymentStatus newStatus, CancellationToken cancellationToken = default);
    }
}
