# CRUD Operations Guide - PRM Car Rental System

This document provides comprehensive CRUD (Create, Read, Update, Delete) operations for all tables in the system using the CQRS pattern with MediatR.

---

## Table of Contents
1. [Users CRUD](#users-crud)
2. [Stations CRUD](#stations-crud)
3. [Vehicles CRUD](#vehicles-crud)
4. [Rentals CRUD](#rentals-crud)
5. [Payments CRUD](#payments-crud)
6. [Implementation Patterns](#implementation-patterns)

---

## Users CRUD

### Entities Involved
- **User**: Main user entity
- **RefreshToken**: JWT token management

### Operations

#### 1. Create User
```
Command: CreateUserCommand
- Input: FullName, Email, Password, Role, DriverLicenseNumber?, IDCardNumber?
- Output: Result<CreateUserResponse>
- Validation: Email unique, Password strength
- Response: User ID, Email, Role
```

#### 2. Get User by ID
```
Query: GetUserByIdQuery
- Input: UserId
- Output: Result<UserDto>
- Response: Full user details including status
```

#### 3. Get All Users
```
Query: GetAllUsersQuery
- Input: PageNumber, PageSize, SortBy, SortOrder
- Output: Result<Page<UserDto>>
- Response: Paginated list with metadata
```

#### 4. Get Users by Role
```
Query: GetUsersByRoleQuery
- Input: Role, PageNumber, PageSize
- Output: Result<Page<UserDto>>
- Response: Filtered users by role with pagination
```

#### 5. Update User
```
Command: UpdateUserCommand
- Input: UserId, FullName, AvatarUrl?, DriverLicenseNumber?
- Output: Result<UpdateUserResponse>
- Validation: User exists, Email unique if changed
- Business Logic: Can't change Role via this command
```

#### 6. Change User Status
```
Command: ChangeUserStatusCommand
- Input: UserId, NewStatus (Active/Inactive/Suspended)
- Output: Result<UserDto>
- Authorization: Admin only
- Business Logic: Can't suspend if user has active rentals
```

#### 7. Verify User Email
```
Command: VerifyUserEmailCommand
- Input: UserId, VerificationToken
- Output: Result<bool>
- Validation: Token valid and not expired
```

#### 8. Delete User (Soft Delete)
```
Command: DeleteUserCommand
- Input: UserId
- Output: Result<bool>
- Authorization: Admin only or self
- Business Logic: Mark as inactive instead of hard delete
```

---

## Stations CRUD

### Entities Involved
- **Station**: Station locations

### Operations

#### 1. Create Station
```
Command: CreateStationCommand
- Input: Name, Address, Latitude, Longitude
- Output: Result<CreateStationResponse>
- Validation: Unique name, Valid coordinates
- Response: Station ID, Name
```

#### 2. Get Station by ID
```
Query: GetStationByIdQuery
- Input: StationId
- Output: Result<StationDto>
- Response: Station details including available vehicles count
```

#### 3. Get All Stations
```
Query: GetAllStationsQuery
- Input: PageNumber, PageSize, SortBy
- Output: Result<Page<StationDto>>
- Response: Paginated stations with vehicle counts
```

#### 4. Search Stations by Location
```
Query: SearchStationsByLocationQuery
- Input: Latitude, Longitude, RadiusKm
- Output: Result<List<StationDto>>
- Business Logic: Haversine formula for distance calculation
- Response: Nearby stations sorted by distance
```

#### 5. Get Available Vehicles in Station
```
Query: GetAvailableVehiclesInStationQuery
- Input: StationId, VehicleType?
- Output: Result<List<VehicleDto>>
- Response: Available vehicles in station, filtered by type if provided
```

#### 6. Update Station
```
Command: UpdateStationCommand
- Input: StationId, Name?, Address?, Latitude?, Longitude?
- Output: Result<StationDto>
- Validation: Unique name if changed
```

#### 7. Delete Station
```
Command: DeleteStationCommand
- Input: StationId
- Output: Result<bool>
- Authorization: Admin only
- Business Logic: Check if no vehicles exist
```

---

## Vehicles CRUD

### Entities Involved
- **Vehicle**: Main vehicle entity
- **VehicleHistory**: Track vehicle movements and maintenance

### Operations

#### 1. Add Vehicle
```
Command: AddVehicleCommand
- Input: PlateNumber, Type, StationId, BatteryLevel
- Output: Result<CreateVehicleResponse>
- Validation: Unique plate number, Valid type
- Response: Vehicle ID, Plate number, Status (Available)
```

#### 2. Get Vehicle by ID
```
Query: GetVehicleByIdQuery
- Input: VehicleId
- Output: Result<VehicleDto>
- Response: Vehicle details including current station and status
```

#### 3. Get All Vehicles
```
Query: GetAllVehiclesQuery
- Input: PageNumber, PageSize, Status?, Type?
- Output: Result<Page<VehicleDto>>
- Response: Filtered vehicles with pagination
```

#### 4. Get Vehicles by Station
```
Query: GetVehiclesByStationQuery
- Input: StationId, PageNumber, PageSize
- Output: Result<Page<VehicleDto>>
- Response: Vehicles at specific station
```

#### 5. Get Available Vehicles
```
Query: GetAvailableVehiclesQuery
- Input: VehicleType, PageNumber, PageSize
- Output: Result<Page<VehicleDto>>
- Filters: Status = Available, BatteryLevel > 20%
- Response: Ready-to-rent vehicles
```

#### 6. Update Vehicle Battery
```
Command: UpdateVehicleBatteryCommand
- Input: VehicleId, BatteryLevel
- Output: Result<VehicleDto>
- Validation: Level 0-100
- Business Logic: Auto-mark maintenance if < 10%
- Side Effect: Record in VehicleHistory
```

#### 7. Change Vehicle Status
```
Command: ChangeVehicleStatusCommand
- Input: VehicleId, NewStatus (Available/InUse/Maintenance/Reserved)
- Output: Result<VehicleDto>
- Validation: Valid state transition
- Side Effect: Record in VehicleHistory
```

#### 8. Move Vehicle to Station
```
Command: MoveVehicleToStationCommand
- Input: VehicleId, TargetStationId
- Output: Result<VehicleDto>
- Business Logic: Update StationId, record history
- Side Effect: Create VehicleHistory entry
```

#### 9. Get Vehicle History
```
Query: GetVehicleHistoryQuery
- Input: VehicleId, PageNumber, PageSize, StartDate?, EndDate?
- Output: Result<Page<VehicleHistoryDto>>
- Response: History of movements and status changes
```

#### 10. Delete Vehicle (Mark as Deleted)
```
Command: DeleteVehicleCommand
- Input: VehicleId
- Output: Result<bool>
- Authorization: Admin only
- Business Logic: Check no active rentals
```

---

## Rentals CRUD

### Entities Involved
- **Rental**: Main rental transaction
- **Vehicle**: Referenced vehicle
- **User**: Renter and Staff
- **Payment**: Associated payments

### Operations

#### 1. Start Rental (Create)
```
Command: StartRentalCommand
- Input: VehicleId, RenterId, StationId, StaffId
- Output: Result<CreateRentalResponse>
- Validation:
  - Vehicle status = Available
  - User (Renter) status = Active and verified
  - Battery > 10%
- Business Logic:
  - Set StartTime = now
  - Status = Active
  - TotalCost = 0
- Side Effects:
  - Change vehicle status to InUse
  - Record in VehicleHistory
- Response: Rental ID, Vehicle details, Start time
```

#### 2. Complete Rental
```
Command: CompleteRentalCommand
- Input: RentalId, EndStationId, FinalBatteryLevel
- Output: Result<CompleteRentalResponse>
- Validation:
  - Rental status = Active
  - EndTime not already set
- Business Logic:
  - Calculate TotalCost (based on duration and vehicle type)
  - Set EndTime = now
  - Status = Completed
  - Move vehicle to EndStationId
  - Update vehicle battery
- Side Effects:
  - Record in VehicleHistory
  - Emit RentalCompletedEvent (for payment processing)
- Response: Rental details, Total cost, Receipt
```

#### 3. Get Rental by ID
```
Query: GetRentalByIdQuery
- Input: RentalId
- Output: Result<RentalDto>
- Response: Complete rental details with related entities
```

#### 4. Get All Rentals
```
Query: GetAllRentalsQuery
- Input: PageNumber, PageSize, Status?, SortBy
- Output: Result<Page<RentalDto>>
- Response: Paginated rentals
```

#### 5. Get User's Rentals
```
Query: GetUserRentalsQuery
- Input: UserId, Status?, PageNumber, PageSize
- Output: Result<Page<RentalDto>>
- Response: Rentals by specific user (as renter)
```

#### 6. Get Active Rentals
```
Query: GetActiveRentalsQuery
- Input: PageNumber, PageSize
- Output: Result<Page<RentalDto>>
- Filters: Status = Active (ongoing rentals)
- Response: Current active rentals
```

#### 7. Cancel Rental
```
Command: CancelRentalCommand
- Input: RentalId, Reason
- Output: Result<bool>
- Validation: Status must be Pending or Active
- Business Logic:
  - Status = Cancelled
  - Return vehicle to Available status
  - Store cancellation reason
- Side Effects: Emit RentalCancelledEvent
```

#### 8. Get Rental Revenue Report
```
Query: GetRentalRevenueReportQuery
- Input: StartDate, EndDate, GroupBy (Daily/Weekly/Monthly)
- Output: Result<List<RevenueReportDto>>
- Response: Revenue metrics for period
```

---

## Payments CRUD

### Entities Involved
- **Payment**: Payment records
- **Rental**: Associated rental

### Operations

#### 1. Record Payment (Create)
```
Command: RecordPaymentCommand
- Input: RentalId, Amount, PaymentMethod
- Output: Result<CreatePaymentResponse>
- Validation:
  - Amount > 0
  - Rental exists
  - Valid payment method
- Business Logic:
  - Set PaidTime = now
  - Payment status = Success (or Pending for certain methods)
- Response: Payment ID, Amount, Method
```

#### 2. Get Payment by ID
```
Query: GetPaymentByIdQuery
- Input: PaymentId
- Output: Result<PaymentDto>
- Response: Payment details with rental reference
```

#### 3. Get All Payments
```
Query: GetAllPaymentsQuery
- Input: PageNumber, PageSize, PaymentMethod?, Status?
- Output: Result<Page<PaymentDto>>
- Response: Paginated payments
```

#### 4. Get Payments by Rental
```
Query: GetPaymentsByRentalQuery
- Input: RentalId
- Output: Result<List<PaymentDto>>
- Response: All payments for specific rental
```

#### 5. Get Payments by User
```
Query: GetPaymentsByUserQuery
- Input: UserId, StartDate?, EndDate?, PageNumber, PageSize
- Output: Result<Page<PaymentDto>>
- Response: User's payment history
```

#### 6. Get Payment Summary
```
Query: GetPaymentSummaryQuery
- Input: StartDate, EndDate, PaymentMethod?
- Output: Result<PaymentSummaryDto>
- Response: 
  - Total payments
  - By payment method breakdown
  - Average payment amount
```

#### 7. Refund Payment
```
Command: RefundPaymentCommand
- Input: PaymentId, Reason
- Output: Result<bool>
- Validation:
  - Payment can be refunded
  - Reason required
- Business Logic:
  - Create reverse transaction
  - Update rental status if needed
```

---

## Implementation Patterns

### 1. Command Handler Template
```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(
        IDbContext dbContext,
        IPasswordHasher passwordHasher,
        IValidator<CreateUserCommand> validator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<Result<CreateUserResponse>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Validation happens automatically via pipeline
        
        var userExists = await _dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);
        
        if (userExists)
            return Result.Failure<CreateUserResponse>(new Error("User.EmailExists", "User with this email already exists"));

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserResponse(user.Id, user.Email));
    }
}
```

### 2. Query Handler Template
```csharp
public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<Page<UserDto>>>
{
    private readonly IDbContext _dbContext;

    public GetAllUsersQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsQueryable();

        // Apply sorting
        query = request.SortBy switch
        {
            "email" => query.OrderBy(u => u.Email),
            "created" => request.SortOrder == SortOrder.Asc 
                ? query.OrderBy(u => u.CreatedAt)
                : query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderBy(u => u.FullName)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        
        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.Status,
                u.CreatedAt))
            .ToListAsync(cancellationToken);

        var page = new Page<UserDto>(
            users,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result.Success(page);
    }
}
```

### 3. Validator Template
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain digit");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}
```

### 4. Controller Endpoint Template
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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "fullName",
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery(pageNumber, pageSize, sortBy);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}
```

### 5. Key Points
- Use `ICommandHandler<TCommand, TResult>` for write operations
- Use `IQueryHandler<TQuery, TResult>` for read operations
- Always return `Result<T>` for consistent error handling
- Use validators for input validation (automatic via pipeline)
- Implement pagination for list queries
- Use async/await for all database operations
- Check authorization and business rules before executing
- Record side effects (history, events) where applicable
- Emit domain events for important business transactions

---

## Additional Considerations

### Business Rules
1. Users must verify email before renting
2. Rentals can only be started with available vehicles
3. Pricing calculation based on vehicle type and rental duration
4. Battery level affects vehicle availability
5. Maintenance status prevents rental
6. User role determines authorization levels

### Performance Optimization
1. Use pagination for large datasets
2. Implement caching for frequently accessed data
3. Use indexes on foreign keys and search fields
4. Consider async operations throughout
5. Use projections in queries to minimize data transfer

### Security
1. Hash passwords never store plain text
2. Validate all user inputs
3. Implement role-based authorization
4. Audit sensitive operations
5. Use JWT for stateless authentication
