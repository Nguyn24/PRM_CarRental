using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Rentals;
using Domain.Users;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rentals.Commands;

public sealed class StartRentalCommandHandler : ICommandHandler<StartRentalCommand, StartRentalResponse>
{
    private readonly IDbContext _dbContext;

    public StartRentalCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<StartRentalResponse>> Handle(StartRentalCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle is null)
            return Result.Failure<StartRentalResponse>(
                Error.NotFound("Vehicle.NotFound", "Vehicle not found"));

        if (vehicle.Status != VehicleStatus.Available)
            return Result.Failure<StartRentalResponse>(
                Error.Conflict("Vehicle.NotAvailable", "Vehicle is not available for rental"));

        if (vehicle.BatteryLevel < 10)
            return Result.Failure<StartRentalResponse>(
                Error.Validation("Vehicle.LowBattery", "Vehicle battery is too low for rental"));

        var renter = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.RenterId, cancellationToken);

        if (renter is null || renter.Status != UserStatus.Active || !renter.IsVerified)
            return Result.Failure<StartRentalResponse>(
                Error.Problem("User.NotEligible", "User is not eligible to rent a vehicle"));

        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            RenterId = request.RenterId,
            StationId = request.StationId,
            StaffId = request.StaffId,
            StartTime = DateTime.UtcNow,
            Status = RentalStatus.Ongoing,
            TotalCost = 0
        };

        vehicle.Status = VehicleStatus.InUse;

        _dbContext.Rentals.Add(rental);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new StartRentalResponse(
            rental.Id,
            vehicle.Id,
            vehicle.PlateNumber,
            rental.StartTime,
            rental.Status.ToString()));
    }
}
