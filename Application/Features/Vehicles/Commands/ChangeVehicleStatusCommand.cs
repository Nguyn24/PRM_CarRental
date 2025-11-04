using Application.Abstraction.Messaging;
using Application.Features.Vehicles.Queries;
using Domain.Common;
using Domain.Vehicles;

namespace Application.Features.Vehicles.Commands;

public sealed record ChangeVehicleStatusCommand(
    Guid VehicleId,
    VehicleStatus Status) : ICommand<VehicleDto>;

