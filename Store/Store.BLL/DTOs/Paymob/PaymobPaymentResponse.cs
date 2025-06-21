using System;
using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.Paymob
{
    public class PaymobPaymentResponse
    {
        public string Token { get; set; }
        public string IframeUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
} 
