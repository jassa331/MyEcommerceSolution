using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] StripeCheckoutRequest req)
    {
        var domain = "https://localhost:7175";        // 🔥 Your Blazor Website Port

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(req.Amount * 100), // ₹ → paise
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = req.ProductName
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = $"{domain}/payment_success?orderId={req.OrderId}",
            CancelUrl = $"{domain}/payment_failed"
        };

        var service = new SessionService();
        Session session = service.Create(options);

        return Ok(new { url = session.Url });
    }
}

public class StripeCheckoutRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string ProductName { get; set; } = "";
}
