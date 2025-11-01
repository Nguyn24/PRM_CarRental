using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Application.Features.Vehicles.Queries;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Commands;

public sealed class ChangeVehicleStatusCommandHandler : ICommandHandler<ChangeVehicleStatusCommand, VehicleDto>
{
    private readonly IDbContext _dbContext;

    public ChangeVehicleStatusCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<VehicleDto>> Handle(ChangeVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.Vehicles
            .Include(v => v.Station)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle is null)
            return Result.Failure<VehicleDto>(
                Error.NotFound("Vehicle.NotFound", $"Vehicle with ID {request.VehicleId} not found"));

        // Validate state transition: Cannot change from InUse to Available if has active rental
        if (vehicle.Status == VehicleStatus.InUse && request.Status == VehicleStatus.Available)
        {
            var hasActiveRental = await _dbContext.Rentals
                .AnyAsync(r => r.VehicleId == request.VehicleId && r.Status == Domain.Rentals.RentalStatus.Ongoing, cancellationToken);

            if (hasActiveRental)
                return Result.Failure<VehicleDto>(
                    Error.Conflict("Vehicle.HasActiveRental", "Cannot change status to Available while vehicle has an active rental"));
        }

        var oldStatus = vehicle.Status;
        vehicle.Status = request.Status;

        // Record vehicle history
        var history = new VehicleHistory
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            TimeStamp = DateTime.UtcNow,
            BatteryLevel = vehicle.BatteryLevel,
            ConditionNote = $"Status changed from {oldStatus} to {request.Status}"
        };

        _dbContext.VehicleHistories.Add(history);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var vehicleDto = new VehicleDto(
            vehicle.Id,
            vehicle.PlateNumber,
            vehicle.Type,
            vehicle.Status.ToString(),
            vehicle.BatteryLevel,
            vehicle.StationId,
            vehicle.Station.Name,
            DateTime.UtcNow); // CreatedAt - placeholder, can be updated if Vehicle has CreatedAt property

        return Result.Success(vehicleDto);
    }
}

