namespace CleanArchitecture.Application.DTOs
{
    public class InitializePaymentRequest
    {
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        // Callback URL is now retrieved from configuration, not supplied by consumers.
    }

    public class InitializePaymentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackData? Data { get; set; }
    }

    public class PaystackData
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }

    public class VerifyPaymentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public VerifyData? Data { get; set; }
    }

    public class VerifyData
    {
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string GatewayResponse { get; set; } = string.Empty;
    }

    // Webhook DTOs
    public class PaystackWebhookRequest
    {
        public string Event { get; set; } = string.Empty;
        public WebhookData? Data { get; set; }
    }

    public class WebhookData
    {
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string GatewayResponse { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public CustomerInfo? Customer { get; set; }
    }

    public class CustomerInfo
    {
        public string Email { get; set; } = string.Empty;
    }

    // Response DTO for payment history
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
