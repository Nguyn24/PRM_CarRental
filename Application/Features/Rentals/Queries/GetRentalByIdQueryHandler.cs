using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rentals.Queries;

public sealed class GetRentalByIdQueryHandler : IQueryHandler<GetRentalByIdQuery, RentalDto>
{
    private readonly IDbContext _dbContext;

    public GetRentalByIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<RentalDto>> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .Include(r => r.Vehicle)
            .Include(r => r.Renter)
            .Include(r => r.Staff)
            .Include(r => r.Station)
            .FirstOrDefaultAsync(r => r.Id == request.RentalId, cancellationToken);

        if (rental is null)
            return Result.Failure<RentalDto>(
                Error.NotFound("Rental.NotFound", $"Rental with ID {request.RentalId} not found"));

        var rentalDto = new RentalDto(
            rental.Id,
            rental.VehicleId,
            rental.Vehicle.PlateNumber,
            rental.RenterId,
            rental.Renter.FullName,
            rental.StaffId,
            rental.Staff.FullName,
            rental.StationId,
            rental.Station.Name,
            rental.StartTime,
            rental.EndTime,
            rental.TotalCost,
            rental.Status.ToString());

        return Result.Success(rentalDto);
    }
}
