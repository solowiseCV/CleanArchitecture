using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class PaymentRepository(ApplicationDbContext context) : IPaymentRepository
    {
        public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
        {
            return await context.Payments
                .FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            // prevent duplicate insertion for the same paystack reference
            if (await context.Payments.AnyAsync(p => p.Reference == payment.Reference, cancellationToken))
            {
                throw new InvalidOperationException("A payment with this reference already exists.");
            }

            await context.Payments.AddAsync(payment, cancellationToken);
        }

        public void Update(Payment payment)
        {
         
            context.Payments.Update(payment);
        }
        public async Task<bool> TryUpdateStatusAsync(string reference, PaymentStatus newStatus, CancellationToken cancellationToken = default)
        {
            var payment = await context.Payments.FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
            if (payment == null || payment.Status == newStatus)
                return false;

            payment.Status = newStatus;
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}
