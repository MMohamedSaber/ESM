# Enterprise Service Management System (ESM)

## Project Description
ESM is a robust backend system designed for a service provider company to seamlessly manage its operations. It enables the company to manage users, employees, customers, departments, services, service requests, appointments, payments, invoices, files, notifications, reviews, reports, and audit logs. 

The primary goal of this project is to serve as a comprehensive **"Backend Playground"** to practice, implement, and master real-world backend concepts, patterns, and technologies.

## Architecture
The project strictly follows **Clean Architecture** to ensure the separation of concerns, testability, and maintainability. The high-level architecture consists of the following layers:

- **ESM.Domain**: The core of the system containing Entities, Value Objects, Enums, Domain Rules, Domain Events, and primary Interfaces.
- **ESM.Application**: Contains the business logic, Use Cases, Commands, Queries (CQRS), DTOs, Validators, Application Services, and Interfaces.
- **ESM.Infrastructure**: Responsible for external concerns such as Data Access (EF Core, DbContext, Repositories), Identity, Redis caching, Email services, File Storage, and Background Jobs.
- **ESM.API**: The application's entry point, which handles HTTP requests, Controllers, Authentication, Middleware, Filters, Swagger documentation, and Global Exception Handling.
- **ESM.Tests**: Contains all Testing layers including Unit Tests, Integration Tests, and E2E Tests.

## Technology Stack
The project leverages a modern and powerful technology stack:

- **Backend Framework**: C#, ASP.NET Core Web API, REST API
- **Data Access & Security**: Entity Framework Core, ASP.NET Core Identity, JWT Authentication, Policy/Role-based Authorization
- **Primary Database**: PostgreSQL
- **Caching**: Redis (Distributed Caching, Cache-aside)
- **Real-Time Communication**: ASP.NET Core SignalR
- **Background Processing**: Hangfire or Quartz.NET
- **Validation**: FluentValidation
- **Logging & Monitoring**: Serilog, Audit Logs, Health Checks
- **API Documentation**: Swagger / OpenAPI
- **Testing**: xUnit, Moq, FluentAssertions, Testcontainers
- **Infrastructure & Deployment**: Docker, Docker Compose, Nginx, GitHub Actions (CI/CD)

## Project Structure
```text
ESM/
├── ESM.slnx                 # Solution File
├── ESM.Domain/              # Core domain entities and rules
├── ESM.Application/         # Application logic, DTOs, and Use Cases
├── ESM.Infrastructure/      # Database access, caching, and external services
├── ESM.API/                 # REST APIs, controllers, and middleware
└── ESM.Tests/               # Unit, integration, and E2E tests
```

## How to Run

1. Ensure you have the [.NET SDK](https://dotnet.microsoft.com/) installed on your machine. For infrastructure dependencies like PostgreSQL and Redis, ensure [Docker](https://www.docker.com/) is installed.
2. Open a terminal in the root directory of the project (where `ESM.slnx` or `docker-compose.yml` is located).
3. If you have a `docker-compose.yml` file ready, spin up the infrastructure (Database, Redis):
   ```bash
   docker-compose up -d
   ```
4. Build the solution to restore dependencies and compile the code:
   ```bash
   dotnet build
   ```
5. Run the API project:
   ```bash
   dotnet run --project ESM.API
   ```
6. Once the application is up and running, you can explore and test the endpoints via the Swagger UI at:
   `https://localhost:<port>/swagger` (check the console output for the exact port).
