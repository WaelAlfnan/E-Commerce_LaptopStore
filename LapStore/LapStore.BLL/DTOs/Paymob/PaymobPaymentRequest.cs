using System;

namespace LapStore.BLL.DTOs.Paymob
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

    public class BillingData
    {
        public string Apartment { get; set; }
        public string Email { get; set; }
        public string Floor { get; set; }
        public string FirstName { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
    }
} 