using System.Threading.Tasks;
using LapStore.BLL.DTOs.Paymob;

namespace LapStore.BLL.Interfaces
{
    public interface IPaymobService
    {
        Task<PaymobPaymentResponse> CreatePaymentRequestAsync(decimal amount, string orderId, BillingData billingData);
        Task<bool> ValidatePaymentCallbackAsync(string hmac, string data);
    }
} 