using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.IService
{
    public interface IPaystackService
    {
        Task<InitializePaymentResponse> InitializeTransaction(InitializePaymentRequest request);
        Task<VerifyPaymentResponse> VerifyTransaction(string reference);
        bool VerifyWebhookSignature(string payload, string paystackSignature);
    }
}
