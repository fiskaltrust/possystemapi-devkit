# HOWTO 08 - Executing the three main actions: Payment, Sign, and Issue in C#

## Prerequisites

Ensure that the **POS System API** and CashBox configuration are set up and ready for fiscal signing. For more information, see the [fiskaltrust documentation](https://docs.fiskaltrust.cloud/).

## What to expect when running this How-To

All How-To's reuse code provided as a .NET assembly/library in source, which can be found in the `../libPosSystemAPI/` directory. After trying out the How-To's, reviewing this code is recommended to gain a better understanding of how it works under the hood and how it interacts with the **POS System API**.

This How-To demonstrates the following:

- Initial credential reading and executing an `/echo` request to verify that the **fiskaltrust POS System API** backend is reachable (handled by `libPosSystemAPI`).
- Creation of a dummy list of charge items.
- Executing the following actions using `ftPosAPIOperationRunner`, which is a helper class that wraps the retry logic:
  1. `/pay` the charge items.
  2. Generate the receipt information using `/sign`.
  3. Deliver the receipt to the user via `/issue`.
- Waiting for the receipt delivered status.
