using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.Payments.Queries;
using MediatR;

namespace CleanArchitecture.Application.Payments.Handlers
{
    public class GetUserTransactionsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<GetUserTransactionsQuery, List<PaymentResponse>>
    {
        public async Task<List<PaymentResponse>> Handle(GetUserTransactionsQuery request, CancellationToken cancellationToken)
        {
            var payments = await unitOfWork.Payments.GetByUserIdAsync(request.UserId, cancellationToken);
            return mapper.Map<List<PaymentResponse>>(payments.ToList());
        }
    }
}
