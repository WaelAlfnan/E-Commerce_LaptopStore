using LapStore.BLL.DTOs.Paymob;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymobService _paymobService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymobService paymobService, ILogger<PaymentController> logger)
        {
            _paymobService = paymobService;
            _logger = logger;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Amount <= 0)
                {
                    return BadRequest(new { message = "Amount must be greater than zero" });
                }

                var billingData = new BillingData
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    City = model.City,
                    Country = model.Country,
                    State = model.State,
                    PostalCode = model.PostalCode
                };

                var response = await _paymobService.CreatePaymentRequestAsync(model.Amount, model.OrderId, billingData);

                if (!response.Success)
                {
                    return BadRequest(new { message = response.ErrorMessage });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for order {OrderId}", model.OrderId);
                return StatusCode(500, new { message = "An error occurred while processing your payment request" });
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("payment-callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Hmac) || string.IsNullOrEmpty(model.Data))
                {
                    return BadRequest(new { message = "Invalid callback data" });
                }

                var isValid = await _paymobService.ValidatePaymentCallbackAsync(model.Hmac, model.Data);

                if (!isValid)
                {
                    _logger.LogWarning("Invalid payment callback received");
                    return BadRequest(new { message = "Invalid payment callback" });
                }

                // Process successful payment
                // Update order status, etc.

                return Ok(new { message = "Payment processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return StatusCode(500, new { message = "An error occurred while processing the payment callback" });
            }
        }
    }
} 