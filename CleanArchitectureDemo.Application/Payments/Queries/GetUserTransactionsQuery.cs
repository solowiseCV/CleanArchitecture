using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Payments.Queries
{
    public record GetUserTransactionsQuery(string UserId) : IRequest<List<PaymentResponse>>;
}
