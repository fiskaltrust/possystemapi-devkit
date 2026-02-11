# HOWTO 02 - Getting started with fiskal signing in C#

## Prerequisites

Ensure that the **POS System API** and CashBox configuration are set up and ready for fiscal signing. For more information, [fiskaltrust documentation](https://docs.fiskaltrust.cloud/).

## What to expect when running this How-To

All How-To's reuse code provided as a .NET assembly/library in source, which can be found in the `../libPosSystemAPI/` directory. After trying out the How-To's, reviewing this code is recommended to gain a better understanding of how it works under the hood and how it interacts with the **POS System API**.

This How-To demonstrates the following:

- Initial credential reading and executing an `/echo` request to verify that the **fiskaltrust POS System API** backend is reachable (handled by `libPosSystemAPI`).
- Creation of a `/sign` request to simulate signing a receipt.
- Writing the result of the request to the console.
- Basic error handling.
