using System;

namespace LapStore.BLL.DTOs.Paymob
{
    public class PaymobPaymentResponse
    {
        public string Token { get; set; }
        public string IframeUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PaymobAuthResponse
    {
        public string Token { get; set; }
    }

    public class PaymobOrderResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
    }
} 