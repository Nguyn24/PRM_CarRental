# PRM Car Rental - Project Structure & Architecture

## Project Overview
This is a **Car Rental Management System** built using **Clean Architecture** with **CQRS (Command Query Responsibility Segregation)** pattern using MediatR. It's a .NET-based backend API for managing car rentals, users, payments, and vehicle tracking.

---

## Architecture Layers

### 1. **Domain Layer** (Domain/)
- **Purpose**: Contains business logic and entities
- **Contains**:
  - **Entities**: User, Vehicle, Rental, Payment, Station, RefreshToken, VehicleHistory
  - **Value Objects**: UserRole, UserStatus, VehicleStatus, VehicleType, RentalStatus, PaymentMethod
  - **Enums**: Statuses and types for various entities
  - **Domain Events**: IDomainEvent interface for event sourcing

### 2. **Application Layer** (Application/)
- **Purpose**: Orchestrates business use cases using CQRS pattern
- **Contains**:
  - **Commands**: Write operations (Create, Update, Delete)
  - **Queries**: Read operations (Get, GetAll, Search)
  - **Handlers**: Command and Query handlers implementing business logic
  - **Validators**: FluentValidation validators for input validation
  - **Abstractions**: Interfaces for authentication, database, messaging, services
  - **Behaviors**: Cross-cutting concerns (Logging, Validation)

### 3. **Infrastructure Layer** (Infrastructure/)
- **Purpose**: Implements data access and external services
- **Contains**:
  - **Database**: ApplicationDbContext, EntityConfigurations, Migrations
  - **Authentication**: JWT token provider, password hasher, user context
  - **Configurations**: EF Core configurations for each entity
  - **Dependency Injection**: Service registrations

### 4. **API Layer** (API/)
- **Purpose**: HTTP entry points and request/response handling
- **Contains**:
  - **Controllers**: REST endpoints for each domain
  - **Middleware**: Exception handling, request logging
  - **Extensions**: Swagger, CORS, Migration configurations
  - **Program.cs**: Application startup configuration

---

## Database Schema

### Core Tables:

#### 1. **Users** 
- `Id` (PK) - Unique identifier
- `FullName` - User's full name
- `Email` - Email address (unique)
- `PasswordHash` - Hashed password
- `Role` - UserRole enum (Admin, Staff, Customer)
- `DriverLicenseNumber` - Driver license ID (optional)
- `IDCardNumber` - ID card number (optional)
- `CreatedAt` - Account creation timestamp
- `Status` - UserStatus enum (Active, Inactive, Suspended)
- `IsVerified` - Email verification flag
- `AvatarUrl` - Profile picture URL (optional)
- **Relations**: RefreshTokens (1:Many), Rentals as Renter (1:Many), Rentals as Staff (1:Many)

#### 2. **Stations**
- `Id` (PK) - Unique identifier
- `Name` - Station name
- `Address` - Station address
- `Latitude` - GPS latitude
- `Longitude` - GPS longitude
- **Relations**: Vehicles (1:Many), Rentals (1:Many)

#### 3. **Vehicles**
- `Id` (PK) - Unique identifier
- `StationId` (FK) - Current station
- `PlateNumber` - License plate (unique)
- `Type` - VehicleType enum (Car, Motorcycle, Bicycle)
- `BatteryLevel` - Battery percentage (0-100)
- `Status` - VehicleStatus enum (Available, InUse, Maintenance, Reserved)
- **Relations**: Station (Many:1), VehicleHistories (1:Many), Rentals (1:Many)

#### 4. **VehicleHistories**
- `Id` (PK) - Unique identifier
- `VehicleId` (FK) - Vehicle reference
- `Action` - History action type
- `Timestamp` - When the action occurred
- `Details` - Additional action details (JSON)
- **Purpose**: Track vehicle movements and maintenance

#### 5. **Rentals**
- `Id` (PK) - Unique identifier
- `VehicleId` (FK) - Rented vehicle
- `RenterId` (FK) - Customer who rented
- `StationId` (FK) - Rental station
- `StaffId` (FK) - Staff member handling rental
- `StartTime` - Rental start time
- `EndTime` - Rental end time (null if ongoing)
- `TotalCost` - Total rental cost
- `Status` - RentalStatus enum (Pending, Active, Completed, Cancelled)
- **Relations**: Vehicle, User (Renter), User (Staff), Station, Payments (1:Many)

#### 6. **Payments**
- `Id` (PK) - Unique identifier
- `RentalId` (FK) - Associated rental
- `Amount` - Payment amount
- `PaymentMethod` - PaymentMethod enum (Cash, Card, MobileMoney, Bank)
- `PaidTime` - Payment timestamp
- **Relations**: Rental (Many:1)

#### 7. **RefreshTokens**
- `Id` (PK) - Unique identifier
- `UserId` (FK) - Associated user
- `Token` - JWT token
- `ExpiresAt` - Token expiration time
- `IsRevoked` - Revocation flag
- **Purpose**: Handle JWT token refresh mechanism

---

## CQRS Pattern

### Commands (Write Operations)
- **Create**: Add new entity
- **Update**: Modify existing entity
- **Delete**: Remove entity
- **Special**: Business logic commands (CompleteRental, StartRental, etc.)

### Queries (Read Operations)
- **GetById**: Fetch single entity
- **GetAll**: Fetch all entities with pagination
- **GetByFilter**: Fetch with specific criteria
- **Special**: Specialized queries (GetAvailableVehicles, GetUserRentals, etc.)

### Handler Pattern
Each command/query has a corresponding handler implementing:
```csharp
ICommandHandler<TCommand, TResult>
IQueryHandler<TQuery, TResult>
```

---

## Key Features

1. **Authentication**: JWT-based with refresh tokens
2. **Authorization**: Role-based access control (Admin, Staff, Customer)
3. **Validation**: FluentValidation with pipeline behaviors
4. **Logging**: Request/response logging middleware
5. **Error Handling**: Global exception handler with custom result format
6. **Database**: Entity Framework Core with migrations
7. **Pagination**: Built-in pagination support for queries
8. **CORS**: Configured for development and production

---

## Technology Stack

- **.NET 8+** - Framework
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **JWT** - Authentication
- **Swagger/OpenAPI** - API documentation
- **SQL Server / PostgreSQL** - Database
- **Docker** - Containerization

---

## Project Structure Summary

```
PRM_CarRental/
├── Domain/                    # Business logic & entities
│   ├── Users/                # User entities and enums
│   ├── Vehicles/             # Vehicle and VehicleHistory
│   ├── Stations/             # Station entity
│   ├── Rentals/              # Rental and RentalStatus
│   ├── Payments/             # Payment and PaymentMethod
│   └── Common/               # Base classes (Entity, Result, Error)
│
├── Application/              # Use cases with CQRS
│   ├── Abstraction/         # Interfaces for CQRS
│   │   ├── Messaging/       # ICommand, IQuery, Handlers
│   │   ├── Data/            # IDbContext
│   │   ├── Authentication/  # Auth interfaces
│   │   └── Services/        # Service contracts
│   └── Features/            # Commands and Queries (to be created)
│
├── Infrastructure/          # Data access & external services
│   ├── Database/            # DbContext, Configurations, Migrations
│   ├── Authentication/      # JWT, Password hashing
│   └── Configuration/       # Entity configurations
│
├── API/                     # HTTP layer
│   ├── Controllers/         # REST endpoints (to be created)
│   ├── Extensions/          # Startup configurations
│   ├── Middleware/          # Request/response processing
│   └── Program.cs           # Application entry point
│
└── PRM_Project_Assignment.sln  # Solution file
```
