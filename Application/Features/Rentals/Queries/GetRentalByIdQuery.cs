using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Rentals.Queries;

public sealed record GetRentalByIdQuery(Guid RentalId) : IQuery<Result<RentalDto>>;

public sealed record RentalDto(
    Guid Id,
    Guid VehicleId,
    string PlateNumber,
    Guid RenterId,
    string RenterName,
    Guid StaffId,
    string StaffName,
    Guid StationId,
    string StationName,
    DateTime StartTime,
    DateTime? EndTime,
    decimal TotalCost,
    string Status);
