using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Commands;

public sealed class AddVehicleCommandHandler : ICommandHandler<AddVehicleCommand, Result<CreateVehicleResponse>>
{
    private readonly IDbContext _dbContext;

    public AddVehicleCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateVehicleResponse>> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        var stationExists = await _dbContext.Stations
            .AnyAsync(s => s.Id == request.StationId, cancellationToken);

        if (!stationExists)
            return Result.Failure<CreateVehicleResponse>(
                new Error("Station.NotFound", $"Station with ID {request.StationId} not found"));

        var plateExists = await _dbContext.Vehicles
            .AnyAsync(v => v.PlateNumber == request.PlateNumber, cancellationToken);

        if (plateExists)
            return Result.Failure<CreateVehicleResponse>(
                new Error("Vehicle.PlateExists", "Vehicle with this plate number already exists"));

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            PlateNumber = request.PlateNumber,
            Type = request.Type,
            StationId = request.StationId,
            BatteryLevel = request.BatteryLevel,
            Status = VehicleStatus.Available
        };

        _dbContext.Vehicles.Add(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateVehicleResponse(
            vehicle.Id,
            vehicle.PlateNumber,
            vehicle.Type,
            vehicle.Status.ToString()));
    }
}
