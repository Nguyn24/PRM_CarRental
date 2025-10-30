using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Queries;

public sealed class GetVehicleByIdQueryHandler : IQueryHandler<GetVehicleByIdQuery, Result<VehicleDto>>
{
    private readonly IDbContext _dbContext;

    public GetVehicleByIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<VehicleDto>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.Vehicles
            .Include(v => v.Station)
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle is null)
            return Result.Failure<VehicleDto>(
                new Error("Vehicle.NotFound", $"Vehicle with ID {request.VehicleId} not found"));

        var vehicleDto = new VehicleDto(
            vehicle.Id,
            vehicle.PlateNumber,
            vehicle.Type,
            vehicle.Status.ToString(),
            vehicle.BatteryLevel,
            vehicle.StationId,
            vehicle.Station.Name,
            DateTime.UtcNow);

        return Result.Success(vehicleDto);
    }
}
