# Title: Using Design Pattern Saga and Fault Management with Polly for Critical Operations

## Context
We are developing a bank transfer system that requires updating the balance of origin and destination accounts, in addition to queries and customer registrations through calls to an external API. The balance update operation consists of four main steps: source account balance query via external API, destination account balance query via external API, source account balance update via external API and balance update of the target account via external API. It is crucial to ensure that, in the event of a failure at any of the steps, account balances are reverted to their original values ​​before the balance operation began. Additionally, we implement retry policies for GET queries and use circuit breaker combined with caching to optimize the performance and reliability of operations.

## Decision
We chose to apply the Design Pattern Saga to coordinate the operation of updating the balance of the source and destination accounts through calls to the external API. Saga is a design pattern that allows you to control distributed transactions where multiple operations need to be coordinated and, in case of failure, changes made so far can be undone. Additionally, we implement retry policies using Polly for GET query operations, combined with circuit breaker and cache to improve the reliability and performance of external API calls.

### Implementation Details with Polly:
- **Retry for GET Queries:** We implement retry policies using Polly for GET query operations to external APIs. This helps deal with temporary network or service failures by retrying the operation after a configurable interval of time.

- **Circuit Breaker + Cache:** We use the circuit breaker in conjunction with a cache mechanism to optimize query performance. When a query is successful, the result is cached. If the circuit breaker enters the OPEN state due to recurring failures, subsequent queries can be served from the cache, avoiding repeated calls to the external API.

## Consequences
By adopting the Saga standard to coordinate account balance updates through external API calls and implementing retry policies with Polly for GET queries, we expect greater robustness and reliability in bank transfer operations and customer management. However, this implies additional complexity in implementing the code, especially in managing compensation operations for each stage of Saga and managing failures with retry and circuit breaker policies.

## Alternatives Considered
We consider other approaches, such as distributed transactions using manual clearing or the use of message queues for asynchronous coordination of operations. We chose the Saga standard due to its ability to maintain financial data consistency even in scenarios of partial failure of external API operations, combined with the performance improvements provided by Polly.

This architectural decision record (ADR) serves as useful documentation for the development team and stakeholders, providing a clear understanding of the reasons behind Saga pattern choices, Polly and circuit breaker + cache retry policies, and their implications for bank transfer operations and customer management in the system under development.
