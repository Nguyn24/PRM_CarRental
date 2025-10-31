using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Stations.Queries;

public sealed class GetStationByIdQueryHandler : IQueryHandler<GetStationByIdQuery, StationDto>
{
    private readonly IDbContext _dbContext;

    public GetStationByIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<StationDto>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        var station = await _dbContext.Stations
            .Include(s => s.Vehicles)
            .FirstOrDefaultAsync(s => s.Id == request.StationId, cancellationToken);

        if (station is null)
            return Result.Failure<StationDto>(
                Error.NotFound("Station.NotFound", $"Station with ID {request.StationId} not found"));

        var availableCount = station.Vehicles.Count(v => v.Status == VehicleStatus.Available);

        var dto = new StationDto(
            station.Id,
            station.Name,
            station.Address,
            station.Latitude,
            station.Longitude,
            availableCount);

        return Result.Success(dto);
    }
}

