# libPosSystemAPI

This is a simple library used by all the C# based howtos to interact with the fiskaltrust POS System API.
Its main purpose are:

- Providing DTOs reflecting the APIs data (note: It only covers the parts needed by the howtos and is incomplete; extend it as needed)
- Implements some classes to manage an `Operation`, a `...Request`, a `...Response`
  - An `Operation` is using a unique `operationID` and contains the request and its response after the operation is executed.
  - An `Operation` is also able to check if there is an error during execution and can provide an easy to access error information.
- Implements a facade `ftPosAPI` that is used to initialize the library and to execute the initial actions (like `EchoAsync`)
