# HOWTO 01 - Getting started with payment in C#

## Prerequisites

Install and configure the **InStore app** to execute payments. For more information, see the [Development Kit README](https://github.com/fiskaltrust/possystemapi-devkit/blob/main/README.MD).

## What to expect when running this How-To

All How-To's reuse code provided as a .NET assembly/library in source, which can be found in the `../libPosSystemAPI/` directory. After trying out the How-To's, reviewing this code is recommended to gain a better understanding of how it works under the hood and how it interacts with the **POS System API**.

This How-To demonstrates the following:

- Initial credential reading and executing an `/echo` request to verify that the **fiskaltrust POS System API** backend is reachable (handled by `libPosSystemAPI`).
- Creation of a `/pay` request with `action: payment`.
- Writing the result of the request to the console.
- Basic error handling as described in the following section.

## Errors to be handled

Several critical error scenarios must be managed carefully to prevent double payments or loss of a payment.

**Example:** A payment request is successfully sent, but the response cannot be received. Possible causes include:

- Internet connectivity is lost while waiting for the response.
- The response is not received within the expected time frame (HTTP timeout), which can be simulated by a device losing internet connectivity during payment execution.

In all of these cases, the solution is to resend the original request using the identical operation ID and request body. The backend will then return the final result of the operation.

## Special case - payment vendor added TIP

In many payment vendor apps, it is possible to add a tip to the payment amount.
**Example flow in a restaurant:**
1. The guest asks for the bill.
2. The waiter triggers to create the bill on the POS (e.g., mobile device).
- The POS on the mobile device pushes the amount (for example 10€) to be paid to the configured payment app (via POS System API and the fiskaltrust InStore App)
- The payment app opens and the waiter hands the mobile device over to the guest
- The guest sees a TIP entry screen and adds a tip of 2€
- The guests now pays the full sum of 12€
- The payment app reports a paid amount of 12€ (including 2€ of tip)
- The fiskaltrust InStore App / POS System API does report back the following in the payment response (see also example in the POS System API docs):
  - 2 pay items
  - Pay item 1: The fully paid amount with the receipt -> 12€
  - Pay item 2: The tip with negative amount -> -2€
  - sum of the 2 pay items is the original requested amount of 10€

NOTE: The tip can be calculated by simply subtracting the requested amount from the paid amount (12-10 = 2€ tip) or alternatively from the 2nd pay item (-2 * -1 = 2€ tip).
