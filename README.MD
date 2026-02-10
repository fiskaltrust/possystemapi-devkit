# fiskaltrust POS System API - Development Kit

## Table of Contents

- [Introduction](#introduction)
  - [Running the howtos](#running-the-howtos)
  - [Before you continue - try it out](#before-you-continue---try-it-out)
- [Architecture and handling of multi terminal setups](#architecture-and-handling-of-multi-terminal-setups)
  - [Component deployment](#component-deployment)
  - [Payment Communication flow - terminalID based routing/filtering](#payment-communication-flow---terminalid-based-routingfiltering)
  - [Payment Communication flow - protocol based routing/filtering](#payment-communication-flow---protocol-based-routingfiltering)
- [Debugging the communication](#debugging-the-communication)
- [Useful followup links](#useful-followup-links)

---

## Introduction

We are focusing on running samples using the fiskaltrust InStore App and POS System API to get you up to speed fast.

**NOTE:** As of today we only focus on integration related to the InStore App and the payment API.

### Running the howtos

So far we only provide HOWTOs written in C#. To run them:

- Take care to have installed `.NET 9` runtime and SDK
- Check the README in the HOWTO you want to use to check for any prerequisites to successfully use that HOWTO
- (optional) Visual Studio or Visual Studio Code for easier handling of the projects/solutions
- when using Visual Studio
  - open the solution file in the HOWTO you want to open and run
- when using Visual Studio Code (VS Code)
  - VS Code debug config should already be in place so do choose the HOWTO to debug from the debug target dropdown in the "RUN AND DEBUG" view 
- alternatively on the command line
  - open a terminal window and navigate to the HOWTO's directory you want to run
  - type: `dotnet run`

### Install and configure the InStore App

- InStore App is [installed](https://docs.fiskaltrust.eu/docs/poscreators/middleware-doc/instore-app/installation-guides) and connected to a CashBox via your fiskaltrust sandbox portal
- the HOWTO(s) use environment variables for configuration
    - `FISKALTRUST_CASHBOX_ID` - can be found in your fiskaltrust portal when configuring the CashBox
    - `FISKALTRUST_CASHBOX_ACCESS_TOKEN` - can be found in your fiskaltrust portal when configuring the CashBox
    - `FISKALTRUST_POS_SYSTEM_API_URL`  
      default is `https://possystem-api-sandbox.fiskaltrust.eu/v2`
- A payment provider is [configured](https://docs.fiskaltrust.eu/docs/poscreators/middleware-doc/instore-app/available-settings#payment-settings)

#### Using the `Dummy Payment Provider` for easier integration (InStore App Developer Mode)

IMPORTANT: This feature is available with InStore App v1.2.8-rc1 and newer.

The InStore App supports a developer mode. When active a hidden `Dummy Payment Provider` can be configured that allows easy integration and testing of different payment success and error scenarios without the need to have any actual functioning payment provider access.

How to enable:
- Enable the Developer Mode in the InStore App
  - Start the InStore App so that you see the home screen
  - At the bottom touch 5 times on the fiskaltrust logo so that the Developer Mode PIN Entry dialog opens
  - Enter the PIN `4242`
  - When successful an info dialog opens that the developer mode is now enabled and the app will exit after OK.
- Configure the `Dummy Payment Provider`  
  - Start the InStore app (again).
  - Open the Settings and navigate to the "Payment settings" section
  - Touch on the "Payment entry" (the first one in the "Payment Settings") and select the `Dummy Payment Provider` which is now visible because the developer mode is active.

How to use the `Dummy Payment Provider`:
- When executing a payment action with `use_auto` (or `test`) protocol payments will now be executed by the `Dummy Payment Provider` visible via the payment progress screen that appears when starting a payment action.
- Any amounts will result with SUCCESS response except the defined special amounts
  - payment request for 30000,10 --> DECLINED
  - payment request for 30000,20 --> TIMEOUT (returned as error message as no other option available yet)
  - payment request for 30000,40 --> CANCELLED BY USER
  - payment request for 30000,50 --> success with added guest tip
  - payment request for 30000,60 --> success after 1min delay
  - payment request for 30000,70 --> success after 3min delay
  - payment request for 30000,80 --> success after 6min delay
  - payment request for 30000,90 --> success but only 15000,50 will be approved

### Before you continue - try it out

As a first step please check the [HOWTO 01 Payment readme](HOWTO_01_Payment_csharp/README.MD) on how to get the initial setup running.
When your setup works please continue here to get a deeper understanding and and check out the other HOWTOs.

## Architecture and handling of multi terminal setups

SCENARIO: The following on a usual restaurant flow where the `POS user` is using a mobile device on which a POS app as well as the InStore App is installed.

### Component deployment

Below are 2 common scenarios how the InStore App is used. Others are of course possible and these only act as examples!

#### Restaurant - mobile ordering scenario

When using 2 devices your setup might look like this:

![](docs/diagrams/out/diagrams/possystemapi_payment_architecture/comp_deploy_restaurant_multi_device.svg)

#### Retail - customer receipt scenario

In this scenario the InStore App is installed on a separate Android device positioned next to the POS.

![](docs/diagrams/out/diagrams/possystemapi_payment_architecture/comp_deploy_retail_customer_interface.svg)

### Payment Communication flow - terminalID based routing/filtering

When using multiple devices it is important to [configure](https://docs.fiskaltrust.eu/docs/poscreators/middleware-doc/instore-app/available-settings#terminal-id-filter) the `terminalID` in the InStore App settings so that each device does use its own, unique in the connected cashbox, terminalID:

![](docs/images/instoreapp_settings_terminalID.png)

When executing requests from the POS it is essential to use the terminalID to route the request(s) to the intended target system.
Here is an example when a user (like a waiter) is using an Android smartphone (terminal 02) to operate the POS app on which also the InStore App is installed and a payment transaction is triggered:

![](docs/diagrams/out/diagrams/possystemapi_payment_architecture/payment_comm_flow.svg)

### Payment Communication flow - protocol based routing/filtering

Besides the terminalID the payment requests are also routed/filtered based on their defined protocol defined in the request itself.
The protocol maps to the [configured payment vendor](https://docs.fiskaltrust.eu/docs/poscreators/middleware-doc/instore-app/available-settings#payment-settings) used on the target device in the InStore App settings.
E.g., using `Viva Wallet SoftPay` maps to the protocol `viva_eft_pos_instore`:

![](docs/images/instoreapp_settings_payment.png)

Message routing rules:

- protocol == `use_auto`: If any payment vendor is configured the request gets accepted.
- protocol == specific protocol like `viva_eft_pos_instore`: The request will only be accepted if the payment vendor supporting this protocol is configured (here `Viva Wallet SoftPay`)

## Debugging the communication

We suggest to use [mitmproxy](https://www.mitmproxy.org/) to put in between your solution and the POS System API to see exactly what requests are going back and forth.

- Please see [mitmproxy.bat](mitmproxy.bat) on how to run it.
- Set `FISKALTRUST_POS_SYSTEM_API_URL` as described in HOWTO_Payment_01 to `http://localhost:8080/v2` (the address the mitmproxy will listen on when using the batch file to start it)

Then all requests from the HOWTOs will go to the mitmproxy where you can view these and the mitmproxy will forward the requests to the POS System API (as configured in the batch file).

![](docs/images/mitmproxy.png)


## Useful followup links

- The [POS System API docs](https://docs.fiskaltrust.cloud/apis/pos-system-api)
  - [pay](https://docs.fiskaltrust.cloud/apis/pos-system-api#tag/SynchronAPI/paths/~1pay/post) requests
- The [InStore App docs](https://docs.fiskaltrust.cloud/docs/poscreators/middleware-doc/instore-app/introduction)
  - How to [install the InStore App](https://docs.fiskaltrust.cloud/docs/poscreators/middleware-doc/instore-app/installation-guides) on your device
  - How to [setup the InStore App](https://docs.fiskaltrust.cloud/docs/poscreators/middleware-doc/instore-app/setup-guide) to work with your fiskaltrust environment
    - For development it might make sense to use the [preview release](https://link.fiskaltrust.eu/downloads/instoreapp/preview) instead of the [stable release](https://link.fiskaltrust.eu/downloads/instoreapp/stable).
- Important readings to avoid setup errors
  - [multiterminal setup](https://docs.fiskaltrust.eu/docs/poscreators/middleware-doc/instore-app/multiterminal-settings) and how to avoid duplicate transactions
