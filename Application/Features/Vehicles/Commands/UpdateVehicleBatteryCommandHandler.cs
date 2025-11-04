using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Application.Features.Vehicles.Queries;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Commands;

public sealed class UpdateVehicleBatteryCommandHandler : ICommandHandler<UpdateVehicleBatteryCommand, VehicleDto>
{
    private readonly IDbContext _dbContext;

    public UpdateVehicleBatteryCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<VehicleDto>> Handle(UpdateVehicleBatteryCommand request, CancellationToken cancellationToken)
    {
        if (request.BatteryLevel < 0 || request.BatteryLevel > 100)
            return Result.Failure<VehicleDto>(
                Error.Validation("Battery.InvalidLevel", "Battery level must be between 0 and 100"));

        var vehicle = await _dbContext.Vehicles
            .Include(v => v.Station)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle is null)
            return Result.Failure<VehicleDto>(
                Error.NotFound("Vehicle.NotFound", $"Vehicle with ID {request.VehicleId} not found"));

        var oldBatteryLevel = vehicle.BatteryLevel;
        vehicle.BatteryLevel = request.BatteryLevel;

        // Auto-mark maintenance if battery too low
        if (request.BatteryLevel < 10 && vehicle.Status == VehicleStatus.Available)
        {
            vehicle.Status = VehicleStatus.Maintenance;
        }
        // Auto-mark available if battery is sufficient and was in maintenance
        else if (request.BatteryLevel >= 10 && vehicle.Status == VehicleStatus.Maintenance)
        {
            vehicle.Status = VehicleStatus.Available;
        }

        // Record vehicle history
        var history = new VehicleHistory
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            TimeStamp = DateTime.UtcNow,
            BatteryLevel = request.BatteryLevel,
            ConditionNote = $"Battery updated from {oldBatteryLevel}% to {request.BatteryLevel}%"
        };

        _dbContext.VehicleHistories.Add(history);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var vehicleDto = new VehicleDto(
            vehicle.Id,
            vehicle.PlateNumber,
            vehicle.Type,
            vehicle.Status.ToString(),
            vehicle.ImageUrl,
            vehicle.BatteryLevel,
            vehicle.StationId,
            vehicle.Station.Name,
            DateTime.UtcNow); // CreatedAt - placeholder

        return Result.Success(vehicleDto);
    }
}

