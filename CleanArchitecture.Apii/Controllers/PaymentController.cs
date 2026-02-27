using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Payments.Commands;
using CleanArchitecture.Application.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/payments")]
    public class PaymentController(IMediator mediator, ICurrentUserService currentUserService, IPaystackService paystackService) : ControllerBase
    {
     
        [HttpPost("initialize")]
        public async Task<ActionResult<InitializePaymentResponse>> Initialize([FromBody] InitializePaymentRequest request)
        {
            var result = await mediator.Send(new InitializePaymentCommand(request));
            if (!result.Status) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("verify/{reference}")]
        public async Task<ActionResult<VerifyPaymentResponse>> Verify(string reference)
        {
            var result = await mediator.Send(new VerifyPaymentCommand(reference));
            if (!result.Status) return BadRequest(result);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            // Read raw body for signature verification
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            var signature = Request.Headers["x-paystack-signature"].FirstOrDefault() ?? string.Empty;

            if (!paystackService.VerifyWebhookSignature(payload, signature))
                return Unauthorized("Invalid webhook signature.");

            var webhookPayload = System.Text.Json.JsonSerializer.Deserialize<PaystackWebhookRequest>(
                payload, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (webhookPayload == null)
                return BadRequest("Invalid webhook payload.");

            await mediator.Send(new HandlePaystackWebhookCommand(webhookPayload));
            return Ok();
        }

        [HttpGet("my-transactions")]
        public async Task<ActionResult<List<PaymentResponse>>> GetMyTransactions()
        {
            var userId = currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await mediator.Send(new GetUserTransactionsQuery(userId));
            return Ok(result);
        }
    }
}
