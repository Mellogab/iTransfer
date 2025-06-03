# iTransfer
This repository includes a bank transfer API as part of a problem solution, following the assumptions:

Design and develop a solution that allows customers to transfer between accounts. This solution needs to be resilient, have high availability and easy to evolve/maintain.
![image](https://github.com/Mellogab/iTransfer/assets/7771245/ec1a07eb-25bb-4646-a2cf-363e1fee9f0b)

### Develop a REST API with the following requirements:

1. Be developed in APS.NET C#/Core Web API language;
2. Validate whether the client who will receive the transfer exists by passing the idCliente in the API
Register;
3. Fetch data from the source account by passing idConta in the Accounts API;
4. Validate whether the current account is active;
5. Validate whether the customer has a balance available in their current account to make the transfer;
6. The Accounts API will return the customer's daily limit if the value is zero or less than the
value of the transfer to be made, the transfer cannot be carried out;
7. Prevent momentary failures of application dependencies from impacting the user experience
client;
8. Documentation
9. Implementation of resilience standards in the application.
10. Architectural drawing.
11. Unit and automated testing
12. Clean Code
13. Error Handling
14. Proposal for architectural improvements
15. Design Patterns

## Overview

This project involves the development of a .NET Core API for managing bank transfers with enterprise-grade resilience patterns. The API exposes an endpoint for transfer operations. It follows RESTful API design principles, Clean Architecture for better project organization and domain isolation, rich domain modeling and advanced fault tolerance mechanisms designed to ensure zero downtime and transactional consistency even during system failures.

## Technologies Used

- **.NET Core 8**: The latest version of .NET Core, offering improved performance and cross-platform support.
- **C#**: Main programming language for back-end development.
- **Entity Framework Core**: Used for database access and object-relational mapping.
- **xUnit**: Unit testing framework used for automated testing.
- **MOQ/AutoMock**: Used to mock classes, interfaces in the test project.
- **Polly**: Used to apply resilience patterns (retry + circuit breaker) to parts of external API query endpoints.
- **Docker**: Used to package and deploy the API as a container.

## Resilience & High Availability Architecture

This API implements **production-grade resilience patterns** to ensure continuous operation and data consistency.

### Saga Pattern with Compensatory Transactions

**Challenge**: Maintaining transactional consistency across multiple account updates when system failures occur.

**Solution Implemented**:
- **Balance Snapshot Strategy**: Before any balance modification, original account values are captured and stored
- **Compensatory Transaction Logic**: On failure detection, automatic rollback restores all accounts to their original states
- **Atomic Operation Tracking**: Each step in the transfer process is monitored within the saga context
- **Consistency Guarantee**: Account states remain consistent even during critical system failures

Success Flow:  Snapshot Balances → Update Source Account → Update Target Account → Commit Transaction
Failure Flow:  Snapshot Balances → Update Source Account → [SYSTEM FAILURE] → Execute Rollback → Restore Original States

### Cache-First Fallback Strategy

**Challenge**: External API dependencies (client validation, account services) causing service unavailability.

**Solution Implemented**:
- **Proactive Cache Population**: Client data and account information cached during normal operations
- **Intelligent Fallback Chain**: Multi-layer protection against external service degradation
- **Zero Downtime Operation**: Service continues functioning even when all external APIs are offline

Resilience Chain:

![image](https://github.com/user-attachments/assets/8a8234e2-77b6-417a-a7b0-f0130d60f5db)


**Result**: 99.9% service availability even with complete external dependency failures.

### Multi-Layer Resilience Protection

#### Transactional Idempotency
- Unique transaction hash generation prevents duplicate processing
- Safe retry capability for clients during network instability
- Automatic duplicate detection and handling

#### Circuit Breaker Pattern
- Configurable failure thresholds for external service protection
- Automatic fallback activation when services degrade
- Self-healing circuit closure when services recover

## Project Structure

The project is divided into four internal projects:

1. **API**: Contains the API controllers.
2. **Core**: Includes use cases, entities and domain logic.
3. **Tests**: Houses unit and integration tests to ensure code quality.
4. **Infrastructure**: Provides the database access layer and dependency configurations.

## Directory Structure


```sh
/iTransfer
  /iTransferencia
    /Controllers
      TranferController.cs
    /Presenters
      DefaultPresenter
    /Configurations
    Program.cs
  /iTransferencia.Core
    /Entities
      /Adapters
        AccountService
        ClientService
        TransferService
      Entity.cs
      Transfer.cs
      Idempotence.cs
    /Enums
      TransferStatus.cs
    /Extensions
      StringHelper.cs
    /Patterns
      ISagaOrquestrator.cs
    /Repository
      IIdempotenceRepository.cs
      IRepository.cs
      IRepositoryBase.cs
      ITransferRepository.cs
    /Services
      IAccountService.cs
      IBacenService.cs
      ICacheService.cs
      IClienteService.cs
      IHttpRequestService.cs
    /UseCases
      /Bacen
        NotifyBacen
          INotifyBacenUseCase.cs
          NotifyBacenUseCaseInput.cs
          NotifyBacenUseCaseOutput.cs
          NotifyBacenUseCase.cs
      /Transfers
        ExecuteTransfer
          IExecuteTransferUseCase.cs
          ExecuteTransferUseCaseInput.cs
          ExecuteTransferUseCaseOutput.cs
          ExecuteTransferUseCase.cs
        UpdateBalances
          IUpdateBalancesUseCase.cs
          UpdateBalancesUseCaseInput.cs
          UpdateBalancesUseCaseOutput.cs
          UpdateBalancesUseCase.cs
    IOutputPort.cs
    IUseCaseRequest.cs
    IUseCaseRequestHandler.cs
    OutputPort.cs
    UseCaseResponseMessage.cs
  /iTransferencia.Infrastructure
    /Automapper
      MappingProfile.cs
    /Configurations
      Retry.cs
      CircuitBreaker.cs
    /DbContext
      ApplicationDbContext
    /EntityFrameworkDataAccess
      /Repositories
        IdempotenceRepository.cs
        Transferrepository.cs
      RepositoryBase.cs
    /Patterns
      /Sagas
        SagaOrquestrator.cs
    /Services
      AccountService.cs
      TransferService.cs
      BacenService.cs
      ClientService.cs
      HttpRequestService.cs
  /iTransferencia.UnitTests
    /Entities
      TransferEntityTests.cs
    /UseCases
      ExecuteUseCaseTests.cs
      NotifyBacenUseCase.cs
      UpdateBalancesUseCaseTests.cs
    /Services
      HttpRequestServiceTests.cs
    /Patterns
      SagaOrquestratorTests.cs
  iTransferencia.sln
  README.md
```

## Suggested Initial Architecture

![image](https://github.com/Mellogab/iTransfer/assets/7771245/f01bef0f-eb05-4bef-be78-4524325d0151)

## Use cases

Use cases define the business logic and are implemented in the Core project. Here are some examples:

- **ExecuteTransferUseCase**: This use case deals with creating a new transfer, querying and updating account balances and notifying the transfer to Bacen.
- **UpdateBalancesUseCase**: This use case manages updating the balance of the source and destination accounts.
- **NotifyBacenUseCase**: This use case manages notification of a transfer made to Bacen.

## Entities

Entities represent domain objects and are defined in the Core project. Here are some examples:

- **Transfer**: Includes properties such as ID, source account, destination account, value and date.
- **Idempotence**: Contains the transactional hash used to identify whether the transfer has already taken place on the same date.

## Tests

The initial 25 tests are implemented in the Tests project and cover use cases, entities, service classes, and design patterns:

![image](https://github.com/Mellogab/iTransfer/assets/7771245/31da294e-580f-4bfa-90fb-67f817ac11bb)

- **Transfer**: Includes properties such as ID, source account, destination account, value and date.
- **Idempotence**: Contains the transactional hash used to identify whether the transfer has already taken place on the same date.

## Infrastructure

The infrastructure project manages database access and dependency configurations. Here are some key points:

- **Entity Framework Core**: Used to map entities to database tables and manage data access.
- **Database**: The database used is SQLServer for local development and can be replaced by a relational database for production.
- **Dependency Injection**: The project uses dependency injection to manage dependencies between classes and modules.
- **Settings**: Environment-specific settings are managed using configuration files.

## Architecture Decision Records

Key architectural decisions can be queried in the /Documentation/Architecture Decision Record folder

- **Title**: Use of Clean Architecture for Bank Transfer API.
- **Title**: Using the Entity Framework to Access the Database.
- **Title**: Using Design Pattern Saga and Fault Management with Polly for Critical Operations.

## Installation instruction
To install and run the project, follow these steps:

1. Clone the repository:
```sh
git clone https://github.com/Mellogab/iTransfer.git
```
2. Navigate to the project folder:
```sh
cd iTransfer
```
3. Restore dependencies:
```sh
dotnet restore
```
4. Configure the connection string in the appsettings.json file in the API folder:
```sh
{
 "ConnectionStrings": {
 "DefaultConnection": "Server=localhost;Database=BancoName;User Id=usuario;Password=password;"
 }
}
```
5. Run the project:
```sh
dotnet run --project iTransferencia
```

## Running the Tests
To run the unit and integration tests, use the following command:

```sh
dotnet test
```

## API endpoints

The API exposes the following endpoints:

Transfers

 POST /api/transfers-async: Send a bank transfer.
