using System.Threading.Tasks;
using Store.BLL.DTOs.Paymob;

namespace Store.BLL.Interfaces
{
    public interface IPaymobService
    {
        Task<PaymobPaymentResponse> CreatePaymentRequestAsync(decimal amount, string orderId, BillingData billingData);
        Task<bool> ValidatePaymentCallbackAsync(string hmac, string data);
    }
} 
