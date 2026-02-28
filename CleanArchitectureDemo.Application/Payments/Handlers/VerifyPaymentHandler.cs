using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Payments.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Payments.Handlers
{
    public class VerifyPaymentHandler(
        IPaystackService paystackService, 
        IUnitOfWork unitOfWork, 
        IUserServices userServices,
        ILogger<VerifyPaymentHandler> logger) 
        : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResponse>
    {
        public async Task<VerifyPaymentResponse> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Verifying payment transaction. Reference: {Reference}", request.Reference);
            var response = await paystackService.VerifyTransaction(request.Reference);

            if (response.Status && response.Data != null && response.Data.Status == "success")
            {
                // atomically update status; repository handles concurrency and returns whether an update occurred
                var updated = await unitOfWork.Payments.TryUpdateStatusAsync(request.Reference, PaymentStatus.Success, cancellationToken);
                if (updated)
                {
                    logger.LogInformation("Payment verified successfully. Status updated and granting premium. Reference: {Reference}", request.Reference);

                    // since the repository saved the change already, we only need to save after updating user
                    await userServices.UpdatePremiumStatusAsync((await unitOfWork.Payments.GetByReferenceAsync(request.Reference, cancellationToken))?.UserId ?? string.Empty, true);
                    await unitOfWork.CompleteAsync(cancellationToken);
                }
                else
                {
                    // either payment not found or already success; log appropriately
                    var existing = await unitOfWork.Payments.GetByReferenceAsync(request.Reference, cancellationToken);
                    if (existing == null)
                        logger.LogWarning("Payment record not found for reference: {Reference}", request.Reference);
                    else
                        logger.LogInformation("Payment already in status '{Status}', no update needed.", existing.Status);
                }
            }
            else
            {
                logger.LogWarning("Payment verification failed or status not success. Reference: {Reference}, Message: {Message}", 
                    request.Reference, response.Message);
            }

            return response;
        }
    }
}
