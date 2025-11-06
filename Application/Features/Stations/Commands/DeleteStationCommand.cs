using Application.Abstraction.Messaging;

namespace Application.Features.Stations.Commands;

public sealed record DeleteStationCommand(Guid StationId) : ICommand<bool>;


