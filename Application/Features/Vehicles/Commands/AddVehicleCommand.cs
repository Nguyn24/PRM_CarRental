using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Vehicles;

namespace Application.Features.Vehicles.Commands;

public sealed record AddVehicleCommand(
    string PlateNumber,
    VehicleType Type,
    Guid StationId,
    int BatteryLevel = 100,
    string? ImageUrl = null) : ICommand<CreateVehicleResponse>;

public sealed record CreateVehicleResponse(
    Guid Id,
    string PlateNumber,
    VehicleType Type,
    string Status,
    string? ImageUrl);
