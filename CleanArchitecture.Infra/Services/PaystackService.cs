using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.IService;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _callbackUrl;

        public PaystackService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Paystack:SecretKey"] ?? throw new ArgumentNullException("Paystack SecretKey is missing");
            _callbackUrl = configuration["Paystack:CallbackUrl"] ?? throw new ArgumentNullException("Paystack CallbackUrl is missing");

            _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<InitializePaymentResponse> InitializeTransaction(InitializePaymentRequest request)
        {
            // Paystack expects amount in Kobo (1 Naira = 100 Kobo)
           
            var body = new
            {
                email = request.Email,
                amount = (int)(request.Amount * 100),
                callback_url = _callbackUrl
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<InitializePaymentResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new InitializePaymentResponse();
        }

        public async Task<VerifyPaymentResponse> VerifyTransaction(string reference)
        {
            var response = await _httpClient.GetAsync($"transaction/verify/{reference}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<VerifyPaymentResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new VerifyPaymentResponse();
        }

        public bool VerifyWebhookSignature(string payload, string paystackSignature)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
            return computedSignature == paystackSignature;
        }
    }
}
