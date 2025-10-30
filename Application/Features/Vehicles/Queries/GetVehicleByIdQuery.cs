using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Vehicles;

namespace Application.Features.Vehicles.Queries;

public sealed record GetVehicleByIdQuery(Guid VehicleId) : IQuery<Result<VehicleDto>>;

public sealed record VehicleDto(
    Guid Id,
    string PlateNumber,
    VehicleType Type,
    string Status,
    int BatteryLevel,
    Guid StationId,
    string StationName,
    DateTime CreatedAt);
