# Decisions
## 2026-09-07 - Stripe Checkout, confirmation by webhook
Payment runs through a page hosted by Stripe. The order reaches `Paid` only on a signed webhook
event.

Not chosen: collecting card data in the API, which puts it in PCI scope. Not chosen: trusting the
browser return URL, which the customer can reach without paying.

Consequence: the webhook endpoint is public, and its only defence is signature verification.