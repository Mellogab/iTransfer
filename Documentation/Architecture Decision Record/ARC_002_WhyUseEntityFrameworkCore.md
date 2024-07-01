# Title: Using the Entity Framework for Database Access

## Context
Our project aims to develop an API to make bank transfers between customer accounts. This API is critical for the business, as it needs to guarantee the integrity of financial transactions, security of customer data and high availability.

## Decision
We chose to use the Entity Framework as ORM (Object-Relational Mapping) to access the database in this project, due to the following considerations:

- **Productivity and Ease of Development:** The Entity Framework simplifies data access through an object model that maps directly to the database schema. This eliminates the need to manually write SQL queries and allows developers to focus more on business logic.

- **Database Abstraction:** The Entity Framework abstracts the database layer, allowing changes to the database schema to be reflected more easily and without significant changes to the application code.

- **Full ORM:** In addition to basic CRUD operations, Entity Framework supports advanced features such as lazy and eager loading, transaction control, and complex mapping of relationships between entities.

## Consequences
By choosing Entity Framework for database access, we expect more agile development that is less prone to errors related to complex SQL queries and database schema maintenance. However, this can introduce performance overhead compared to more direct approaches such as Dapper, especially in operations that require high performance and fine-grained control over the queries executed.

## Alternatives Considered
We considered other alternatives for accessing the database, such as:

- **Direct Access (Pure SQL):** Write SQL queries directly using low-level libraries for greater control and performance.

- **Dapper:** A micro ORM that offers better performance in scenarios where high performance and direct control over executed queries are required.

We chose the Entity Framework due to its integration with the .NET ecosystem, ease of use, and support for complex object-relational mapping operations.

This architectural decision record (ADR) serves as useful documentation for the development team and stakeholders, providing a clear understanding of the reasons behind the choice of Entity Framework and its implications for the product management project in the email system. commerce.
