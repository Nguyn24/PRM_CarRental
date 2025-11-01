using Application.Abstraction.Messaging;
using Application.Features.Vehicles.Queries;
using Domain.Common;

namespace Application.Features.Vehicles.Commands;

public sealed record UpdateVehicleBatteryCommand(
    Guid VehicleId,
    int BatteryLevel) : ICommand<VehicleDto>;

