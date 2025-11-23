using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] StripeCheckoutRequest req)
    {
        StripeConfiguration.ApiKey = "sk_test_51SSAQMBOxzp7UNACemUl8X68qvF8bywcPV87XnqZ9ssbQfcHQaQpquwNslptpjWCd6fOsU726IwIGsUVdFP0axBg00kpqo6W1l";

        // Your website domain
        var domain = "https://localhost:7291";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(req.Amount * 100),  // ₹ → paise
                    Currency = "inr",                       // IMPORTANT
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = req.ProductName
                    }
                },
                Quantity = 1
            }
        },
            Mode = "payment",
            SuccessUrl = $"{domain}/user_orders",
            CancelUrl = $"{domain}/cancel"
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Ok(new { url = session.Url });
    }

    //[HttpPost("create-checkout-session")]
    //public IActionResult CreateCheckoutSession()
    //{
    //    StripeConfiguration.ApiKey = "sk_test_51SSAQMBOxzp7UNACemUl8X68qvF8bywcPV87XnqZ9ssbQfcHQaQpquwNslptpjWCd6fOsU726IwIGsUVdFP0axBg00kpqo6W1l";

    //    var options = new SessionCreateOptions
    //    {
    //        PaymentMethodTypes = new List<string> { "card" },
    //        Mode = "payment",
    //        SuccessUrl = "https://localhost:7291/user_orders",
    //        CancelUrl = "https://localhost:7291/cancel",
    //        LineItems = new List<SessionLineItemOptions>
    //    {
    //        new SessionLineItemOptions
    //        {
    //            PriceData = new SessionLineItemPriceDataOptions
    //            {
    //                Currency = "usd",
    //                UnitAmount = 1000,
    //                ProductData = new SessionLineItemPriceDataProductDataOptions
    //                {
    //                    Name = "Test Product"
    //                }
    //            },
    //            Quantity = 1
    //        }
    //    }
    //    };

    //    var service = new SessionService();
    //    var session = service.Create(options);

    //    return Ok(new { url = session.Url });
    //}
    //    [HttpPost("create-checkout-session")]
    //    public IActionResult CreateCheckoutSession([FromBody] StripeCheckoutRequest req)
    //    {
    //        var domain = "https://localhost:7175";        // 🔥 Your Blazor Website Port

    //        var options = new SessionCreateOptions
    //        {
    //            PaymentMethodTypes = new List<string> { "card" },
    //            LineItems = new List<SessionLineItemOptions>
    //            {
    //                new SessionLineItemOptions
    //                {
    //                    PriceData = new SessionLineItemPriceDataOptions
    //                    {
    //                        UnitAmount = (long)(req.Amount * 100), // ₹ → paise
    //                        Currency = "inr",
    //                        ProductData = new SessionLineItemPriceDataProductDataOptions
    //                        {
    //                            Name = req.ProductName
    //                        }
    //                    },
    //                    Quantity = 1
    //                }
    //            },
    //            Mode = "payment",
    //            SuccessUrl = $"{domain}/payment_success?orderId={req.OrderId}",
    //            CancelUrl = $"{domain}/payment_failed"
    //        };

    //        var service = new SessionService();
    //        Session session = service.Create(options);

    //        return Ok(new { url = session.Url });
    //    }
    //}

    public class StripeCheckoutRequest
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string ProductName { get; set; } = "";
    }
}