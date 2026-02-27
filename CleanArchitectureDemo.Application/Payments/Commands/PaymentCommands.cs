using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using MediatR;

namespace CleanArchitecture.Application.Payments.Commands
{
    public record InitializePaymentCommand(InitializePaymentRequest Request) : IRequest<InitializePaymentResponse>;
    public record VerifyPaymentCommand(string Reference) : IRequest<VerifyPaymentResponse>;
    public record HandlePaystackWebhookCommand(PaystackWebhookRequest Payload) : IRequest<bool>;
}
