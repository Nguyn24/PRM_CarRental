using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Queries;

public sealed class GetVehiclesByStationQueryHandler : IQueryHandler<GetVehiclesByStationQuery, Page<VehicleDto>>
{
    private readonly IDbContext _dbContext;

    public GetVehiclesByStationQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<VehicleDto>>> Handle(GetVehiclesByStationQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Vehicles
            .Include(v => v.Station)
            .Where(v => v.StationId == request.StationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(v => v.PlateNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VehicleDto(
                v.Id,
                v.PlateNumber,
                v.Type,
                v.Status.ToString(),
                v.BatteryLevel,
                v.StationId,
                v.Station.Name,
                DateTime.UtcNow))
            .ToListAsync(cancellationToken);

        var page = new Page<VehicleDto>(items, request.PageNumber, request.PageSize, totalCount);
        return Result.Success(page);
    }
}

