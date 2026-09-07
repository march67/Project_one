# Vision

Status: Draft
Date: 2026-09-07
Owner: David

## Problem

A single florist shop has no way to sell online. Orders are taken by phone or in person, the
catalogue is not published anywhere, and the owner tracks pending orders on paper.

This project delivers the backend that lets the shop publish its catalogue, accept customer
orders online, and give the owner a single view of the work to prepare.

## Users

Shop manager

- Maintains the product catalogue: create, update, remove, set prices.
- Sees all incoming orders in one dashboard.
- Moves an order through its preparation states.
- Consults the order history.

Customer

- Browses the published catalogue.
- Creates an account and signs in.
- Builds a cart and places an order.
- Pays online at checkout.
- Follows the state of an order.
- Consults personal order history.
- Cancels an order the manager has not started preparing.

## Primary scenario

This sequence, executed end to end against the deployed API, is the acceptance demonstration:

1. A customer registers and signs in.
2. The customer lists the catalogue and reads one product.
3. The customer adds a product to the cart and places the order.
4. The customer pays, and the order moves to a paid state.
5. The manager signs in and sees the order in the pending list.
6. The manager moves the order to preparing, then to ready.
7. The customer reads the current state of the order.

## Success criteria

The project succeeds when all four hold:

- The primary scenario passes end to end as an automated integration test.
- The API is deployed on Azure and reachable over HTTPS.
- The OpenAPI document describes every endpoint and is browsable.
- A cancellation attempt on an order already being prepared is rejected with a documented error.

## In scope

- Authentication and accounts for both roles.
- Product catalogue with prices.
- Cart and order placement.
- Online payment through an external provider.
- Order state lifecycle, advanced manually by the manager.
- Order cancellation by the customer before preparation starts.
- Order history for both roles.
- Manager dashboard data: pending orders, order history.

## Out of scope

Nothing below is built in this version, and no design effort is spent preparing for it.

- Multiple shops or tenants. This version serves one shop.
- Stock and availability. A product can always be ordered.
- Refunds, credit notes, and payment reversal.
- Messaging between customer and manager.
- Delivery addresses, delivery slots, delivery fees, and carrier integration.
- Load handling beyond a single shop's normal traffic.
- Email or push notifications.
- Bouquet composition from individual flowers. A product is an indivisible catalogue item.
- Promotions, discount codes, loyalty.
- The web front end. It is a separate project, started after this one.

## Non-functional constraints

- Expected traffic is one shop's normal activity. No load testing, no scaling work.
- Personal data is limited to what an order requires: name, email, phone.
- Payment card data never reaches this API. The payment provider handles it.

## Technical constraints

These are imposed, not chosen during the project.

- .NET and C#.
- SQL Server, accessed through an ORM.
- API and database packaged with Docker, dependencies included.
- Deployed on Azure.

## Timeline

- Duration: 3 weeks.
- Weekly effort available: TO FILL.
- Hard deadline: TO FILL.

## Risks

R1 Payment integration exceeds its estimate
Trigger: provider webhooks, payment states, and local testing take longer than 3 days.
Mitigation: fall back to a simulated payment gateway, keeping the payment states in the model.
Decision point: end of week 2.

R2 Azure deployment discovered too late
Trigger: deployment attempted in the final days and blocked by configuration.
Mitigation: deploy an empty API to Azure in week 1, before any feature exists.

R3 Scope grows during implementation
Trigger: a feature not listed in scope gets built because it seemed small.
Mitigation: the out of scope list is the reference; changing it happens only at the weekly review.

## Future direction

Recorded so it is not confused with scope. None of this constrains the current design.

- Serving several artisan shops from the same API.
- A web front end for customers and for the manager.
- Delivery management.
