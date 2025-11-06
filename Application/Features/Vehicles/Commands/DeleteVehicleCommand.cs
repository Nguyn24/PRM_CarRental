using Application.Abstraction.Messaging;

namespace Application.Features.Vehicles.Commands;

public sealed record DeleteVehicleCommand(Guid VehicleId) : ICommand<bool>;


