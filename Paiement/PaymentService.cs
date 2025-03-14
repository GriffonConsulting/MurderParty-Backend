﻿using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Session CreateSession(string email, Guid userId, Product[] products, Guid[] productsIds)
        {
            List<SessionLineItemOptions> lineItems = [];
            var paymentIntentMetadata = new Dictionary<string, string>
                {
                   { "ProductsIds" ,string.Join(",", productsIds) },
                   { "UserId", userId.ToString() }
                };

            foreach (var product in products)
            {
                lineItems.Add(new()
                {
                    Price = product.PriceCode,
                    Quantity = 1
                });
            }

            var options = new SessionCreateOptions
            {
                InvoiceCreation = new SessionInvoiceCreationOptions
                {
                    Enabled = true,
                    InvoiceData = new SessionInvoiceCreationInvoiceDataOptions
                    {

                        RenderingOptions = new SessionInvoiceCreationInvoiceDataRenderingOptionsOptions
                        {
                            AmountTaxDisplay = "include_inclusive_tax",
                        },
                        Footer = _configuration["Stripe:Footer"],
                    },
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = paymentIntentMetadata
                },
                UiMode = "embedded",
                Locale = "fr",
                CustomerEmail = email,

                BillingAddressCollection = "required",
                LineItems = lineItems,
                Mode = "payment",
                ReturnUrl = _configuration["FrontEndUrl"] + "/account",

            };
            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }
    }
}
