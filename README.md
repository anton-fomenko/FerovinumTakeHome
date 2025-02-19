# Ferovinum Take-Home Assignment

## Overview

This repository implements a **CLI** for selling and buying stock (wine/whisky), matching the user prompt. The app stores all historical orders and reprint them after each new command, updating statuses accordingly (i.e., "remaining:X" or "closed").

I chose to implement the **Command Pattern** to handle the core domain operations (`Sell`, `Buy`). Each command class encapsulates the logic specific to that operation, making the code modular and easy to extend with new operations (e.g., future "transfer", "reserve").

To demonstrate maintainability, I've placed the direct instantiation of commands in `FerovinumProcessor` with a **Factory** that accepts token arrays. This design further decouples the parsing logic from the domain logic, making it easier to add new commands with distinct parameter patterns.

## Quick Start / Usage

1. **.NET CLI** (locally):
   - `dotnet build`
   - `dotnet run --project FerovinumTakeHome`
   - Then enter commands, for example:
     ```
     sell wine 1000
     sell whisky 100
     buy wine 500
     ...
     ```
   - After each command, the system reprints the entire updated order history, including the newly updated or closed orders.

2. **Docker**:
   - `docker build -t ferovinum-cmd .`
   - `docker run -it ferovinum-cmd`
   - The container runs `Program.Main()`, allowing interactive input.

## Running Tests

Using xUnit in the `FerovinumTakeHome.Tests` project:

- `dotnet test`
   
## Requirements Met

1. **CLI interface** that accepts lines:
   - `sell [sku] [quantity]`
   - `buy [sku] [quantity]`
2. **Print** the entire history after each command, with updated statuses (partial or fully closed orders).
3. **No frameworks**: I only use the standard libraries (e.g., `System.Collections.Generic`).
4. **Docker**: I included a sample Dockerfile in the root of the repository.
5. **Tests**:
   - I have unit tests for each command.
   - An integration test replicating the sample sequence from the PDF.

## Edge Cases and Assumptions

- **Invalid Commands**: If the command does not match the required `sell/buy sku quantity` format, the system ignores it and prints the existing history.
- **Negative or zero quantity**: Also ignored. The system is meant to run continuously without errors.
- **Unsupported SKU**: The list of allowed SKUs is stored in FerovinumContext object. Only "wine" and "whiskey" allowed. Everything else is ignored.
- **Concurrent Access**: Current version of the app is not concurrency-safe. A typical approach in a multi-user environment would be to store orders in a database or lock shared data.
- **Use of BuyOrder and SellOrder**: I decided to extend an OrderBase class for polymorphism. I assume BuyOrder doesn't need a Remaining property, while SellOrder does. This respects SOLID principles by giving each type only the fields and logic it requires.
## Object Interaction

Below is how the main objects in the system interact:

- **Program**: The entry point of the application. It continuously reads lines from STDIN, passes them to the `FerovinumProcessor`, then prints out the updated order history.

- **FerovinumProcessor**: Interprets the user input by splitting tokens and delegating to the **CommandFactory** to create the right command (`SellCommand`, `BuyCommand`). After executing the command, it returns the entire list of orders in the system.

- **CommandFactory**: Parses the tokens (params) and instantiates the correct `IFerovinumCommand`.

- **IFerovinumCommand** / **SellCommand**, **BuyCommand**: Each command implements logic for how the system’s state changes. `SellCommand` creates new sell orders and enqueues them, while `BuyCommand` consumes from the earliest queued sell orders.

- **FerovinumContext**: The in-memory representation of all orders and data structures used to track them (e.g., queues of sell orders). Commands operate on this context.

- **OrderBase**: A domain object stored in `FerovinumContext`. Each order tracks an original quantity and SKU.
- **SellOrder**: Inherits from OrderBase, but has its own unique "Remaining" property.
- **BuyOrder**: Inherits from OrderBase, but overrides the ToString() method that describes it properly. BuyOrder is always "Closed" when instantiated.

## Why .NET?

I chose **.NET** for this solution because of the following advantages:

- **Familiarity & Expertise**: With years of experience in .NET, I can ensure a well-structured, maintainable, and performant implementation.
- **Robust Standard Library**: .NET provides built-in data structures (`List<T>`, `Dictionary<K,V>`, `Queue<T>`) that suit this problem well.
- **Strong Typing & Safety**: The static type system helps prevent runtime errors and ensures maintainability.
- **Cross-Platform**: .NET Core allows seamless execution on Windows, Linux, and macOS, making deployment flexible.
- **Built-in Unit Testing**: xUnit integrates smoothly with .NET, ensuring robust testing without additional dependencies.
- **Docker Support**: .NET applications containerize efficiently with minimal overhead, making it easy to meet the assignment's Docker requirement.

## Why Use the Command Pattern?

- **Extensibility**: If Ferovinum adds new operations (e.g., "reserve", "transfer", "lease"), I simply implement a new `IFerovinumCommand`. This avoids growing a monolithic `switch` or `if/else`.
- **Modularity**: Each command object (`SellCommand`, `BuyCommand`) has a single responsibility.
- **Testability**: I can easily unit-test each command in isolation. The integration test checks the entire flow.

## Why Use a Factory Pattern?

- **Reduced Conditional Logic in Processor**: The processor only splits the input into tokens; the factory decides how to interpret them for each command. This keeps `FerovinumProcessor` small and focused on orchestrating the workflow.
- **Command Extensibility**: If new commands have more parameters or a different ordering, you only update the factory logic.


## Data Structures

- **List<Order>**: Chronological list of all orders (both Sell and Buy) for reprinting the entire history.
- **Dictionary<string, Queue<Order>>**: For each SKU, we store Sell orders in a FIFO queue, ensuring earliest stock is used first.

## Future Extensions and Production Readiness

To make this solution production-ready, I would:

- **Introduce a Repository/Database Layer**: Instead of storing orders in an in-memory list/queue, implement a repository pattern that persists orders to a relational or NoSQL database. This ensures durability and fault tolerance.
- **Handle Concurrency**: If multiple clients can run commands in parallel, we’d ensure proper isolation and consistency. In a database context, this might involve row-level locks or optimistic concurrency checks on the orders table.
- **Expand Domain Logic**: Our command-based approach already supports easy additions. For instance:
  - A `TransferCommand` or `ReserveCommand` can be created without altering existing command classes.
  - Each new command can integrate with the same shared `FerovinumContext` or repository.
- **Advanced Validation**: We could add domain services or validators that check business rules (e.g., ensuring total capacity constraints, restricting client buybacks beyond certain thresholds, etc.).
- **Logging and Monitoring**: In production, we’d incorporate robust logging, metrics, and potentially trace each command for debugging or auditing.