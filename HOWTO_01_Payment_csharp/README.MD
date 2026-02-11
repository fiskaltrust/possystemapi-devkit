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
