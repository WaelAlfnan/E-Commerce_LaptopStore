using System;

namespace LapStore.BLL.DTOs.Paymob
{
    public class PaymentCallbackModel
    {
        public string Hmac { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
} 