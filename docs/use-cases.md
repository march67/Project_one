# Use cases
Scope reference: `docs/vision.md`.

This file holds what the system must do. Each use case carries a stable identifier used in the work item title and in
the commit message.

Each acceptance criterion is written so it becomes one integration test.

## Shared rules
Order states

```
PendingPayment -> Paid -> Preparing -> Ready -> Completed
PendingPayment -> Cancelled
Paid           -> Cancelled
```

- An order state only moves forward, except to `Cancelled`.
- Only the manager moves an order from `Paid` onward.
- A customer cancels only from `PendingPayment` or `Paid`.
- Once an order reaches `Preparing`, it can no longer be cancelled.

Pricing

- An order line stores the product price as it was when the order was placed.
- Changing a product price never changes an existing order.

Availability

- No stock is tracked. A published product can always be ordered.

Ownership

- A customer reads and cancels only their own orders.
- The manager reads every order.

## Accounts
### UC-001 Sign in with a Google account
As a visitor, I want to sign in with my Google account so that I can order without creating a
password.

The system stores no password. Google is the only way in, for customers and for the manager alike.
An account is created on first sign-in, so there is no separate registration.

- A valid Google identity that matches no account creates one and returns an access token.
- A valid Google identity that matches an existing account signs into it.
- The access token carries the user identity and role.
- A new account receives the customer role and no manager permission.
- A created account stores the email and the display name from the Google identity.
- An invalid, expired or wrongly issued Google identity is rejected.
- A request to a protected endpoint without a token is rejected as unauthenticated.
- A customer token is rejected on a manager endpoint.

### UC-002 Renew an access token
As a signed-in user, I want my session to continue without signing in again.

- A valid refresh token returns a new access token.
- A refresh token already used is rejected.

## Catalogue, manager side
### UC-010 Create a product

As the manager, I want to add a product so that customers can order it.

- A product with a name, a description, a price and a published flag is created.
- A product with a price of zero or below is rejected.
- A product with a name already used is rejected as a conflict.
- A created product is readable by its identifier.

### UC-011 Update a product

As the manager, I want to correct a product so that the catalogue stays accurate.

- Updating name, description or price changes the product.
- Updating the price of a product does not change any existing order line.
- Updating a product that does not exist is rejected as not found.
- A price of zero or below is rejected.

### UC-012 Withdraw a product from the catalogue

As the manager, I want to withdraw a product so that it can no longer be ordered.

- A withdrawn product no longer appears in the customer catalogue.
- A withdrawn product remains readable on the orders that already contain it.
- Ordering a withdrawn product is rejected.

## Catalogue, customer side
### UC-020 List the catalogue

As a visitor, I want to browse the products so that I can choose one.

- The catalogue returns only published products.
- The catalogue is paginated, and the response carries the total count.
- The catalogue is readable without being signed in.

### UC-021 Read a product

As a visitor, I want to read a product so that I can decide to order it.

- A published product returns its name, description and current price.
- An unknown identifier is rejected as not found.
- A withdrawn product is rejected as not found.

## Ordering
### UC-030 Place an order

As a customer, I want to order the products I selected so that the shop prepares them.

The cart is built client side and submitted in one payload. No cart entity exists in the domain,
and no use case covers cart persistence.

- An order with at least one line and positive quantities is created in `PendingPayment`.
- The order total is the sum of its lines, computed from the prices at placement time.
- An order containing an unknown or withdrawn product is rejected.
- An order with an empty line list is rejected.
- An order with a quantity of zero or below is rejected.
- The created order belongs to the signed-in customer.

### UC-031 Start the payment of an order

As a customer, I want to be sent to the payment page so that I can pay for my order.

Payment runs through Stripe Checkout, a page hosted by the provider. See `docs/decisions.md`.

- Starting the payment of an order in `PendingPayment` returns the hosted checkout URL.
- The checkout session carries the order lines and the total computed at placement time.
- Starting the payment of an order that is not in `PendingPayment` is rejected.
- Starting the payment of an order owned by another customer is rejected.
- Starting the payment twice on the same order returns a usable checkout URL both times.

### UC-032 Confirm a payment

As the system, I want to record the provider's confirmation so that the order state is trustworthy.

An order reaches `Paid` only on a signed webhook event, never on the browser redirection that
follows the payment page.

- A confirmation event with a valid signature moves the matching order from `PendingPayment` to `Paid`.
- A confirmation event with an invalid signature is rejected and changes nothing.
- A confirmation event already processed, identified by its event identifier, is ignored.
- A confirmation event for an unknown order is acknowledged and changes nothing.
- A confirmation event for an order that is not in `PendingPayment` changes nothing.
- A failed or expired payment event leaves the order in `PendingPayment`.

### UC-033 Cancel an order

As a customer, I want to cancel an order the shop has not started so that I am not charged for it.

- Cancelling an order in `PendingPayment` moves it to `Cancelled`.
- Cancelling an order in `Paid` moves it to `Cancelled`.
- Cancelling an order in `Preparing`, `Ready` or `Completed` is rejected.
- Cancelling an order owned by another customer is rejected.

### UC-034 Follow an order

As a customer, I want to read the state of my order so that I know when it is ready.

- Reading an owned order returns its state, its lines and its total.
- Reading an order owned by another customer is rejected.
- Reading an unknown order is rejected as not found.

### UC-035 List my orders

As a customer, I want the list of my orders so that I can find a past one.

- The list returns only the signed-in customer's orders, most recent first.
- The list is paginated, and the response carries the total count.
- A customer with no order receives an empty list, not an error.

## Order management
### UC-040 List orders to prepare

As the manager, I want the orders waiting for work so that I know what to prepare.

- The list returns the orders in `Paid`, `Preparing` and `Ready`, oldest first.
- Each entry carries the order identifier, the customer name, the lines and the state.
- The list is refused to a customer.

### UC-041 Advance an order

As the manager, I want to move an order forward so that the customer follows its progress.

- Moving an order from `Paid` to `Preparing` succeeds.
- Moving an order from `Preparing` to `Ready` succeeds.
- Moving an order from `Ready` to `Completed` succeeds.
- Moving an order backwards is rejected.
- Skipping a state is rejected.
- Moving a `Cancelled` order is rejected.

### UC-042 List the order history

As the manager, I want the past orders so that I can consult the shop's activity.

- The list returns the orders in `Completed` and `Cancelled`, most recent first.
- The list is paginated, and the response carries the total count.
- The list is refused to a customer.
