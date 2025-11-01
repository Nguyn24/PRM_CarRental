using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rentals.Queries;

public sealed class GetUserRentalsQueryHandler : IQueryHandler<GetUserRentalsQuery, Page<RentalDto>>
{
    private readonly IDbContext _dbContext;

    public GetUserRentalsQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<RentalDto>>> Handle(GetUserRentalsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            return Result.Failure<Page<RentalDto>>(
                Error.Validation("UserRentals.UserIdRequired", "UserId is required"));

        var query = _dbContext.Rentals
            .Include(r => r.Vehicle)
            .Include(r => r.Renter)
            .Include(r => r.Staff)
            .Include(r => r.Station)
            .Where(r => r.RenterId == request.UserId);

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rentals = await query
            .OrderByDescending(r => r.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RentalDto(
                r.Id,
                r.VehicleId,
                r.Vehicle.PlateNumber,
                r.RenterId,
                r.Renter.FullName,
                r.StaffId,
                r.Staff.FullName,
                r.StationId,
                r.Station.Name,
                r.StartTime,
                r.EndTime,
                r.TotalCost,
                r.Status.ToString()))
            .ToListAsync(cancellationToken);

        var page = new Page<RentalDto>(
            rentals,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(page);
    }
}

