using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Payments.Commands;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Payments.Handlers
{
    public class InitializePaymentHandler(IPaystackService paystackService, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<InitializePaymentHandler> logger) 
        : IRequestHandler<InitializePaymentCommand, InitializePaymentResponse>
    {
        public async Task<InitializePaymentResponse> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Initializing payment for user. Amount: {Amount}", request.Request.Amount);
            var response = await paystackService.InitializeTransaction(request.Request);

            if (response.Status && response.Data != null)
            {
                var userId = currentUserService.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("Payment initialization failed: User not authenticated.");
                    return new InitializePaymentResponse { Status = false, Message = "User not authenticated" };
                }

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Amount = request.Request.Amount,
                    Reference = response.Data.Reference,
                    Status = "Pending",
                    TransactionDate = DateTime.UtcNow
                };

                try
                {
                    await unitOfWork.Payments.AddAsync(payment, cancellationToken);
                    await unitOfWork.CompleteAsync(cancellationToken);
                    logger.LogInformation("Payment record created. Reference: {Reference}, UserId: {UserId}", response.Data.Reference, userId);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("reference"))
                {
                    // duplicate reference – idempotent already created by another request
                    logger.LogWarning("Duplicate payment reference detected during initialization: {Reference}", response.Data.Reference);
                }
            }
            else
            {
                logger.LogError("Paystack initialization failed: {Message}", response.Message);
            }

            return response;
        }
    }
}
