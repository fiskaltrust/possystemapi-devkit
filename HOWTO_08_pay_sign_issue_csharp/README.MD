# HOWTO 08 - Execute the 3 main actions - Payment - Sign - Issue in C#

## Prerequisites

Have the POS System API + Cashbox config set up and ready to go for fiskal signing. See [fiskaltrust documentation](https://docs.fiskaltrust.cloud/) for further details.

## What to expect when running this HOWTO

All our howtos are reusing some code that is provided as a .NET Assembly/library in source and can be found in `../libPosSystemAPI/` directory. Please check it after trying out the HOWTOs to get a better understanding what it does under the hood and how it interacts with the POS System API.

The HOWTO demonstrates

- initial credential reading and executing an `/echo` request to know if the fiskaltrust / POS System API backend is reachable (actually done by libPosSystemAPI)
- Creating a dummy list of charge items
- Execute the following actions with ftPosAPIOperationRunner which is a helper class wrapping the retry logic
  - `/pay` the charge items
  - Generate the receipt info with `/sign`
  - Deliver the receipt to the user via `/issue`
- Wait for receipt delivered status

