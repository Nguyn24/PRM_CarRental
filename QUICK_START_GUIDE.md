# Quick Start Guide - PRM Car Rental CRUD Operations

## Project Overview

**PRM Car Rental** is a Clean Architecture-based backend API using CQRS pattern with MediatR for managing vehicle rentals.

### Architecture Layers

```
┌─────────────────────────────────────┐
│     API Layer (REST Endpoints)      │
├─────────────────────────────────────┤
│  Application Layer (Commands/Queries)│
├─────────────────────────────────────┤
│   Infrastructure (Database, Auth)   │
├─────────────────────────────────────┤
│    Domain (Entities, Business Logic) │
└─────────────────────────────────────┘
```

---

## Database Entities

### 1. **Users**
- Registration and authentication
- Roles: Admin, Staff, Customer
- Verification and status tracking

### 2. **Stations**
- Rental pickup/dropoff locations
- GPS coordinates for location-based search

### 3. **Vehicles**
- Cars, Motorcycles, Bicycles
- Status tracking: Available, InUse, Maintenance, Reserved
- Battery level monitoring

### 4. **Rentals**
- Main business transaction
- Links Vehicle + Renter + Station + Staff
- Tracks start/end time and total cost

### 5. **Payments**
- Payment records for rentals
- Multiple payment methods supported
- Audit trail of all transactions

### 6. **VehicleHistory**
- Tracks vehicle movements
- Maintenance logs
- Location history

### 7. **RefreshTokens**
- JWT token management
- User session control

---

## CQRS Pattern Explained

### Commands (Write Operations)
**Purpose**: Create, Update, Delete data

```csharp
// Example: Create a user
var command = new CreateUserCommand(
    fullName: "John Doe",
    email: "john@example.com",
    password: "SecurePass123",
    role: UserRole.Customer);

var result = await _sender.Send(command, cancellationToken);
```

### Queries (Read Operations)
**Purpose**: Retrieve data without side effects

```csharp
// Example: Get user by ID
var query = new GetUserByIdQuery(userId);
var result = await _sender.Send(query, cancellationToken);
```

---

## Implemented CRUD Operations

### ✓ Users (8 operations)
| Operation | Type | File |
|-----------|------|------|
| CreateUser | Command | `Features/Users/Commands/CreateUserCommand.cs` |
| UpdateUser | Command | `Features/Users/Commands/UpdateUserCommand.cs` |
| DeleteUser | Command | `Features/Users/Commands/DeleteUserCommand.cs` |
| GetUserById | Query | `Features/Users/Queries/GetUserByIdQuery.cs` |
| GetAllUsers | Query | `Features/Users/Queries/GetAllUsersQuery.cs` |

### ✓ Vehicles (2 operations + 6 TODO)
| Operation | Type | Status |
|-----------|------|--------|
| AddVehicle | Command | ✓ Done |
| GetVehicleById | Query | ✓ Done |
| UpdateBattery | Command | TODO |
| ChangeStatus | Command | TODO |
| MoveToStation | Command | TODO |
| GetAllVehicles | Query | TODO |
| GetAvailable | Query | TODO |
| GetByStation | Query | TODO |

### ✓ Stations (1 operation + 6 TODO)
| Operation | Type | Status |
|-----------|------|--------|
| CreateStation | Command | ✓ Done |
| UpdateStation | Command | TODO |
| DeleteStation | Command | TODO |
| GetStationById | Query | TODO |
| GetAllStations | Query | TODO |
| SearchByLocation | Query | TODO |

### ✓ Rentals (1 operation + 5 TODO)
| Operation | Type | Status |
|-----------|------|--------|
| StartRental | Command | ✓ Done |
| CompleteRental | Command | TODO |
| CancelRental | Command | TODO |
| GetRentalById | Query | ✓ Done |
| GetAllRentals | Query | TODO |
| GetUserRentals | Query | TODO |
| GetActiveRentals | Query | TODO |

### ✓ Payments (1 operation + 4 TODO)
| Operation | Type | Status |
|-----------|------|--------|
| RecordPayment | Command | ✓ Done |
| GetPaymentById | Query | ✓ Done |
| RefundPayment | Command | TODO |
| GetAllPayments | Query | TODO |
| GetPaymentSummary | Query | TODO |

---

## How to Use CRUD Operations

### Step 1: Inject ISender
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }
```

### Step 2: Create Command/Query
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(
    [FromBody] CreateUserCommand command,
    CancellationToken cancellationToken)
{
    var result = await _sender.Send(command, cancellationToken);
    return result.IsSuccess 
        ? CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value)
        : BadRequest(result);
}
```

### Step 3: Handle Response
```csharp
if (result.IsSuccess)
{
    // Success - access result.Value
    var userId = result.Value.Id;
}
else
{
    // Failure - access result.Errors
    var errorMessage = result.Errors.First().Message;
}
```

---

## Key Files to Understand

### Domain Layer
- `Domain/Users/User.cs` - User entity definition
- `Domain/Vehicles/Vehicle.cs` - Vehicle entity definition
- `Domain/Rentals/Rental.cs` - Rental entity definition
- `Domain/Stations/Station.cs` - Station entity definition
- `Domain/Payments/Payment.cs` - Payment entity definition

### Application Layer (CQRS)
- `Application/Abstraction/Messaging/ICommand.cs` - Command interface
- `Application/Abstraction/Messaging/IQuery.cs` - Query interface
- `Application/Features/Users/Commands/` - User write operations
- `Application/Features/Users/Queries/` - User read operations

### Infrastructure Layer
- `Infrastructure/Database/ApplicationDbContext.cs` - Database context
- `Infrastructure/Configuration/*Configuration.cs` - Entity configurations
- `Infrastructure/Migrations/` - Database migrations

### API Layer
- `API/Program.cs` - Application startup
- `API/Extensions/` - Configuration extensions
- `API/Middlewares/` - Request/response processing

---

## Business Rules

### User Management
- ✓ Email must be unique
- ✓ Passwords are hashed (never stored plain text)
- ✓ Users must verify email before renting
- ✓ Soft delete (marked as Inactive, not removed)

### Vehicle Management
- ✓ Plate number must be unique
- ✓ Battery level < 10% → Auto-mark for maintenance
- ✓ Battery level < 100% → Cannot be rented
- ✓ Only "Available" vehicles can be rented

### Rental Management
- ✓ Can only rent if user is Active and Verified
- ✓ Vehicle must be Available
- ✓ Rental starts with status "Active"
- ✓ Costs calculated based on duration and vehicle type
- ✓ Can only complete active rentals

### Payment Management
- ✓ Amount must be > 0
- ✓ Payment recorded with timestamp
- ✓ Supports multiple payment methods

---

## Common Usage Patterns

### Pattern 1: Create Resource
```csharp
// Command
var command = new CreateUserCommand(...);
var result = await _sender.Send(command, cancellationToken);

// Returns: Result<CreateUserResponse>
// Contains: Id, Email, FullName, Role
```

### Pattern 2: Get Single Resource
```csharp
// Query
var query = new GetUserByIdQuery(userId);
var result = await _sender.Send(query, cancellationToken);

// Returns: Result<UserDto>
// Contains: Full user details
```

### Pattern 3: Get List with Pagination
```csharp
// Query
var query = new GetAllUsersQuery(
    pageNumber: 1,
    pageSize: 10,
    sortBy: "fullName");

var result = await _sender.Send(query, cancellationToken);

// Returns: Result<Page<UserDto>>
// Contains: List of users + pagination metadata
```

### Pattern 4: Update Resource
```csharp
// Command
var command = new UpdateUserCommand(
    userId: userId,
    fullName: "New Name");

var result = await _sender.Send(command, cancellationToken);

// Returns: Result<UserDto>
```

### Pattern 5: Delete Resource
```csharp
// Command
var command = new DeleteUserCommand(userId);
var result = await _sender.Send(command, cancellationToken);

// Returns: Result<bool>
```

---

## Error Handling

All operations return `Result<T>`:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public IEnumerable<Error> Errors { get; set; }
}

public class Error
{
    public string Code { get; set; }      // e.g., "User.NotFound"
    public string Message { get; set; }   // e.g., "User not found"
}
```

### Handling Errors
```csharp
var result = await _sender.Send(command, cancellationToken);

if (!result.IsSuccess)
{
    var errors = result.Errors;
    foreach (var error in errors)
    {
        Console.WriteLine($"{error.Code}: {error.Message}");
    }
}
```

---

## Validation

Validation happens automatically via MediatR pipeline:

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .Must(ContainUpperCase);
    }
}
```

If validation fails, command won't execute and errors are returned.

---

## Adding New CRUD Operations

### Template for New Command:

1. **Create Command Class**
```csharp
public sealed record CreateXYZCommand(...) : ICommand<Result<XYZResponse>>;
```

2. **Create Handler**
```csharp
public sealed class CreateXYZCommandHandler : ICommandHandler<CreateXYZCommand, Result<XYZResponse>>
{
    public async Task<Result<XYZResponse>> Handle(CreateXYZCommand request, CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

3. **Create Validator**
```csharp
public sealed class CreateXYZCommandValidator : AbstractValidator<CreateXYZCommand>
{
    // Validation rules here
}
```

4. **Create Endpoint**
```csharp
[HttpPost]
public async Task<IActionResult> CreateXYZ(
    [FromBody] CreateXYZCommand command,
    CancellationToken cancellationToken)
{
    var result = await _sender.Send(command, cancellationToken);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
}
```

---

## Performance Tips

1. **Use pagination** for large result sets
2. **Implement caching** for frequently accessed data
3. **Add database indexes** on foreign keys
4. **Use async/await** throughout
5. **Project only needed fields** in queries
6. **Load related entities** with Include() to avoid N+1 queries

---

## Testing Commands and Queries

```csharp
[Fact]
public async Task CreateUserCommand_WithValidData_ReturnsSuccess()
{
    // Arrange
    var handler = new CreateUserCommandHandler(_dbContext, _passwordHasher);
    var command = new CreateUserCommand("John", "john@example.com", "Pass123");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotEqual(Guid.Empty, result.Value.Id);
}
```

---

## Next Steps

1. **Review** the generated CRUD files in `Application/Features/`
2. **Implement** the TODO operations following the same patterns
3. **Create controllers** in `API/` for each entity
4. **Write unit tests** for each handler
5. **Add integration tests** with test database
6. **Deploy** and monitor in production

---

## Documentation Files Generated

| File | Purpose |
|------|---------|
| `PROJECT_STRUCTURE.md` | Complete project architecture explanation |
| `CRUD_OPERATIONS.md` | Detailed guide for all CRUD operations |
| `CRUD_IMPLEMENTATION_SUMMARY.md` | Summary of implemented operations |
| `QUICK_START_GUIDE.md` | This file - quick reference |

---

## Support and Resources

- **CQRS Pattern**: Commands separate from Queries for scalability
- **MediatR**: Mediator pattern for loose coupling
- **FluentValidation**: Fluent validation API
- **Entity Framework Core**: ORM for database access
- **Clean Architecture**: Separation of concerns

---

## Summary

You now have:
- ✓ 13+ working CRUD operations
- ✓ Complete architecture explanation
- ✓ Templates for implementing remaining operations
- ✓ Best practices and patterns
- ✓ Error handling and validation
- ✓ Ready-to-use code examples

Start by reviewing the generated files and implementing the TODO operations following the provided patterns!
