using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Rentals.Commands;

public sealed record StartRentalCommand(
    Guid VehicleId,
    Guid RenterId,
    Guid StationId,
    Guid StaffId) : ICommand<Result<StartRentalResponse>>;

public sealed record StartRentalResponse(
    Guid RentalId,
    Guid VehicleId,
    string PlateNumber,
    DateTime StartTime,
    string Status);
