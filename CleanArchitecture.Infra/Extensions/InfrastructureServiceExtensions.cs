using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Resilience;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace CleanArchitecture.Infrastructure.Extensions
{
   
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {
           
            services.AddHttpClient<IPaystackService, PaystackService>()
                .AddPolicyHandler((serviceProvider, _) =>
                {
                    var logger = serviceProvider
                        .GetRequiredService<ILogger<PaystackService>>();

                    var combined = Policy.WrapAsync(
                        PaystackResiliencePolicies.GetFallbackPolicy(logger),
                        PaystackResiliencePolicies.GetCircuitBreakerPolicy(logger),
                        PaystackResiliencePolicies.GetRetryPolicy(logger),
                        PaystackResiliencePolicies.GetTimeoutPolicy()
                    );

                    return combined;
                });

            return services;
        }
    }
}
