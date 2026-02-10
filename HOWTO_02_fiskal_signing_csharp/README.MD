# HOWTO 02 - Getting started with fiskal signing in C#

## Prerequisites

Have the POS System API + Cashbox config set up and ready to go for fiskal signing. See [fiskaltrust documentation](https://docs.fiskaltrust.cloud/) for further details.

## What to expect when running this HOWTO

All our howtos are reusing some code that is provided as a .NET Assembly/library in source and can be found in `../libPosSystemAPI/` directory. Please check it after trying out the HOWTOs to get a better understanding what it does under the hood and how it interacts with the POS System API.

The HOWTO demonstrates

- initial credential reading and executing an `/echo` request to know if the fiskaltrust / POS System API backend is reachable (actually done by libPosSystemAPI)
- creates a `/sign` request to similate signing a receipt
- writes the result of that request to the console
- basic error handling

