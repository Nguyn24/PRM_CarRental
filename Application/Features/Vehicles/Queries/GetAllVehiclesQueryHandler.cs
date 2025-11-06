using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Queries;

public sealed class GetAllVehiclesQueryHandler : IQueryHandler<GetAllVehiclesQuery, Page<VehicleDto>>
{
    private readonly IDbContext _dbContext;

    public GetAllVehiclesQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<VehicleDto>>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Vehicles
            .Where(v => !v.IsDeleted)
            .Include(v => v.Station)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(v => v.Status == request.Status.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(v => v.Type == request.Type.Value);
        }

        query = request.SortBy?.ToLower() switch
        {
            "battery" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(v => v.BatteryLevel)
                : query.OrderByDescending(v => v.BatteryLevel),
            "type" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(v => v.Type)
                : query.OrderByDescending(v => v.Type),
            "status" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(v => v.Status)
                : query.OrderByDescending(v => v.Status),
            _ => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(v => v.PlateNumber)
                : query.OrderByDescending(v => v.PlateNumber)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VehicleDto(
                v.Id,
                v.PlateNumber,
                v.Type,
                v.Status.ToString(),
                v.ImageUrl,
                v.BatteryLevel,
                v.StationId,
                v.Station.Name,
                DateTime.UtcNow))
            .ToListAsync(cancellationToken);

        var page = new Page<VehicleDto>(items, totalCount, request.PageNumber, request.PageSize);
        return Result.Success(page);
    }
}
