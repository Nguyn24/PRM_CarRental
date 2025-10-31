using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Stations.Queries;

public sealed class GetAllStationsQueryHandler : IQueryHandler<GetAllStationsQuery, Page<StationDto>>
{
    private readonly IDbContext _dbContext;

    public GetAllStationsQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<StationDto>>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Stations.Include(s => s.Vehicles).AsQueryable();

        query = request.SortBy?.ToLower() switch
        {
            "address" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(s => s.Address)
                : query.OrderByDescending(s => s.Address),
            _ => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(s => s.Name)
                : query.OrderByDescending(s => s.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StationDto(
                s.Id,
                s.Name,
                s.Address,
                s.Latitude,
                s.Longitude,
                s.Vehicles.Count(v => v.Status == VehicleStatus.Available)))
            .ToListAsync(cancellationToken);

        var page = new Page<StationDto>(items, request.PageNumber, request.PageSize, totalCount);
        return Result.Success(page);
    }
}

