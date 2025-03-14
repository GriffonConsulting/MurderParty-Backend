﻿namespace Application.Invoices.Queries.GetInvoices
{
    public record GetInvoicesResult
    {
        public required decimal Amount { get; init; }
        public required DateTime CreatedOn { get; init; }
        public required string ReceiptUrl { get; init; }
    }
}
