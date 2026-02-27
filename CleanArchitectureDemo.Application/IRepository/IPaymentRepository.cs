using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.IRepository
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
        void Update(Payment payment);

        /// <summary>
        /// Atomically attempts to set a new status for a payment. Returns true if the update occurred.
        /// </summary>
        Task<bool> TryUpdateStatusAsync(string reference, string newStatus, CancellationToken cancellationToken = default);
    }
}
