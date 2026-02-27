using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Payments.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Payments.Handlers
{
    public class HandlePaystackWebhookHandler(
        IPaystackService paystackService, 
        IUnitOfWork unitOfWork, 
        IUserServices userServices,
        ILogger<HandlePaystackWebhookHandler> logger)
        : IRequestHandler<HandlePaystackWebhookCommand, bool>
    {
        public async Task<bool> Handle(HandlePaystackWebhookCommand request, CancellationToken cancellationToken)
        {
            var payload = request.Payload;
            logger.LogInformation("Processing Paystack webhook. Event: {Event}", payload.Event);

            // Only handle successful charge events
            if (payload.Event != "charge.success" || payload.Data == null)
            {
                logger.LogDebug("Ignoring webhook event: {Event}", payload.Event);
                return false;
            }

            // try to atomically flip status
            var updated = await unitOfWork.Payments.TryUpdateStatusAsync(payload.Data.Reference, "Success", cancellationToken);

            if (!updated)
            {
                var existing = await unitOfWork.Payments.GetByReferenceAsync(payload.Data.Reference, cancellationToken);
                if (existing == null)
                {
                    logger.LogWarning("Webhook received for non-existent payment reference: {Reference}", payload.Data.Reference);
                }
                else if (existing.Status == "Success")
                {
                    logger.LogInformation("Idempotency: Webhook already processed for reference: {Reference}", payload.Data.Reference);
                }
                else
                {
                    logger.LogWarning("Failed to update payment status for reference {Reference}.", payload.Data.Reference);
                }
                return false;
            }

            logger.LogInformation("Webhook verified payment success. Reference: {Reference}, Amount: {Amount}", 
                payload.Data.Reference, payload.Data.Amount);

            // after repository already saved the change, update premium and commit
            var payment = await unitOfWork.Payments.GetByReferenceAsync(payload.Data.Reference, cancellationToken);
            if (payment != null)
            {
                await userServices.UpdatePremiumStatusAsync(payment.UserId, true);
                await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogInformation("User premium status updated via webhook. UserId: {UserId}", payment.UserId);
            }
            return true;

            return true;
        }
    }
}
