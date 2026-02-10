# HOWTO 01 - Getting started with payment in C#

## Prerequisites

Install and configure the InStore app to execute payments (see main DevKit README for further details).

## What to expect when running this HOWTO

All our howtos are reusing some code that is provided as a .NET Assembly/library in source and can be found in `../libPosSystemAPI/` directory. Please check it after trying out the HOWTOs to get a better understanding what it does under the hood and how it interacts with the POS System API.

The HOWTO demonstrates

- initial credential reading and executing an `/echo` request to know if the fiskaltrust / POS System API backend is reachable  (actually done by libPosSystemAPI)
- creates a `/pay` request with `action: payment`
- writes the result of that request to the console
- basic error handling (see next)

## Errors to be handled

There are multiple critical issues to be handled nicely to avoid double payments or the loss of a payment.

### We were able to send the payment request but we were not able to receive the response 

- Because we lost internet connectivity while waiting
- Because we could not get the response in time (= http timeout waiting for the response which can be simulated by a device on which the payment gets executed looses internet connectivity during operation)
- ?

In all cases the solution is to resend the original request (with identical operation ID and request body). The backend will then return the final result.