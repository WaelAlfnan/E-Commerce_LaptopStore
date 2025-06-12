namespace LapStore.BLL.Constants
{
    public static class PaymobConstants
    {
        public const string Currency = "EGP";
        public const int PaymentExpirationInSeconds = 3600;
        public const string PaymentCallbackRoute = "api/payment/payment-callback";
        public const string CreatePaymentRoute = "api/payment/create-payment";
    }
} 