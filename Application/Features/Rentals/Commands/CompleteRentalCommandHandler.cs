using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Rentals;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rentals.Commands;

public sealed class CompleteRentalCommandHandler : ICommandHandler<CompleteRentalCommand, CompleteRentalResponse>
{
    private readonly IDbContext _dbContext;

    public CompleteRentalCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CompleteRentalResponse>> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == request.RentalId, cancellationToken);

        if (rental is null)
            return Result.Failure<CompleteRentalResponse>(
                Error.NotFound("Rental.NotFound", $"Rental with ID {request.RentalId} not found"));

        if (rental.Status != RentalStatus.Ongoing)
            return Result.Failure<CompleteRentalResponse>(
                Error.Conflict("Rental.InvalidStatus", "Rental is not in Ongoing status"));

        if (request.FinalBatteryLevel < 0 || request.FinalBatteryLevel > 100)
            return Result.Failure<CompleteRentalResponse>(
                Error.Validation("Battery.InvalidLevel", "Battery level must be between 0 and 100"));

        var station = await _dbContext.Stations
            .FirstOrDefaultAsync(s => s.Id == request.EndStationId, cancellationToken);

        if (station is null)
            return Result.Failure<CompleteRentalResponse>(
                Error.NotFound("Station.NotFound", $"Station with ID {request.EndStationId} not found"));

        var vehicle = rental.Vehicle;
        var endTime = DateTime.UtcNow;
        var duration = endTime - rental.StartTime;
        
        // Calculate cost based on duration and vehicle type
        // Simplified pricing: 50k/hour for Car, 30k/hour for Scooter, 20k/hour for Other
        var hourlyRate = vehicle.Type switch
        {
            VehicleType.Car => 50000m,
            VehicleType.Scooter => 30000m,
            VehicleType.Other => 20000m,
            _ => 30000m
        };

        var hours = (decimal)Math.Max(1, duration.TotalHours);
        var totalCost = hourlyRate * hours;

        // Update rental
        rental.EndTime = endTime;
        rental.TotalCost = totalCost;
        rental.Status = RentalStatus.Completed;
        rental.StationId = request.EndStationId;

        // Update vehicle
        vehicle.Status = VehicleStatus.Available;
        vehicle.BatteryLevel = request.FinalBatteryLevel;
        vehicle.StationId = request.EndStationId;

        // Auto-mark maintenance if battery too low
        if (request.FinalBatteryLevel < 10)
        {
            vehicle.Status = VehicleStatus.Maintenance;
        }

        // Record vehicle history
        var history = new VehicleHistory
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            TimeStamp = endTime,
            BatteryLevel = request.FinalBatteryLevel,
            ConditionNote = request.Notes
        };

        _dbContext.VehicleHistories.Add(history);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CompleteRentalResponse(
            rental.Id,
            endTime,
            totalCost,
            rental.Status.ToString()));
    }
}

