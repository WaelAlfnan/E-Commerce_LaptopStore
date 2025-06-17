using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LapStore.BLL.Constants;
using LapStore.BLL.DTOs.Paymob;
using LapStore.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace LapStore.BLL.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymobService> _logger;
        private readonly string _apiKey;
        private readonly int _integrationId;
        private readonly string _hmacSecret;
        private readonly string _baseUrl;
        private readonly string _iframeId;

        public PaymobService(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<PaymobService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["Paymob:PAYMOB_API_KEY"] ?? throw new ArgumentNullException("Paymob:PAYMOB_API_KEY");
            _integrationId = int.Parse(_configuration["Paymob:PAYMOB_INTEGRATION_ID"] ?? throw new ArgumentNullException("Paymob:PAYMOB_INTEGRATION_ID"));
            _hmacSecret = _configuration["Paymob:PAYMOB_HMAC_SECRET"] ?? throw new ArgumentNullException("Paymob:PAYMOB_HMAC_SECRET");
            _baseUrl = _configuration["Paymob:BaseUrl"] ?? throw new ArgumentNullException("Paymob:BaseUrl");
            _iframeId = _configuration["Paymob:PAYMOB_IFRAME_ID"] ?? throw new ArgumentNullException("Paymob:PAYMOB_IFRAME_ID");
        }

        public async Task<PaymobPaymentResponse> CreatePaymentRequestAsync(decimal amount, string orderId, BillingData billingData)
        {
            try
            {
                _logger.LogInformation("Creating payment request for order {OrderId} with amount {Amount}", orderId, amount);

                // Step 1: Get Authentication Token
                var authToken = await GetAuthTokenAsync();

                // Step 2: Create Order
                var orderResponse = await CreateOrderAsync(authToken, amount, orderId);

                // Step 3: Create Payment Key
                var paymentKey = await CreatePaymentKeyAsync(authToken, orderResponse.Id, amount, billingData);

                _logger.LogInformation("Payment request created successfully for order {OrderId}", orderId);

                return new PaymobPaymentResponse
                {
                    Token = paymentKey,
                    IframeUrl = $"{_baseUrl}/acceptance/iframes/{_iframeId}?payment_token={paymentKey}",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment request for order {OrderId}", orderId);
                return new PaymobPaymentResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred while processing your payment request"
                };
            }
        }

        public async Task<bool> ValidatePaymentCallbackAsync(string hmac, string data)
        {
            try
            {
                _logger.LogInformation("Validating payment callback");
                using var hmacsha512 = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
                var hash = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(data));
                var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();
                var isValid = calculatedHmac == hmac;

                _logger.LogInformation("Payment callback validation result: {IsValid}", isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating payment callback");
                return false;
            }
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var request = new { api_key = _apiKey };
            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/auth/tokens",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<PaymobAuthResponse>(content);
            return authResponse?.token ?? throw new InvalidOperationException("Failed to get authentication token");
        }

        private async Task<PaymobOrderResponse> CreateOrderAsync(string authToken, decimal amount, string orderId)
        {
            var request = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(amount * 100),
                currency = PaymobConstants.Currency,
                merchant_order_id = orderId
            };

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/ecommerce/orders",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            );


            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var orderResponse = JsonSerializer.Deserialize<PaymobOrderResponse>(content);
            return orderResponse ?? throw new InvalidOperationException("Failed to create order");
        }

        private async Task<string> CreatePaymentKeyAsync(string authToken, int orderId, decimal amount, BillingData billingData)
        {
            var request = new PaymobPaymentRequest
            {
                AuthToken = authToken,
                AmountCents = (int)(amount * 100),
                Expiration = PaymobConstants.PaymentExpirationInSeconds,
                OrderId = orderId.ToString(),
                BillingData = billingData,
                IntegrationId = _integrationId
            };

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/acceptance/payment_keys",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var paymentKeyResponse = JsonSerializer.Deserialize<PaymobPaymentResponse>(content);
            return paymentKeyResponse?.Token ?? throw new InvalidOperationException("Failed to create payment key");
        }
    }
} 