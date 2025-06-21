using System;
using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.Paymob
{
    public class PaymobPaymentRequest
    {
        public string AuthToken { get; set; }
        public int AmountCents { get; set; }
        public int Expiration { get; set; }
        public string OrderId { get; set; }
        public BillingData BillingData { get; set; }
        public string Currency { get; set; } = "EGP";
        public int IntegrationId { get; set; }
    }
} 
