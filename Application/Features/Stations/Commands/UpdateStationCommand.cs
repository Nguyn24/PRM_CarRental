using Application.Abstraction.Messaging;

namespace Application.Features.Stations.Commands;

public sealed record UpdateStationCommand(
    Guid StationId,
    string? Name,
    string? Address,
    decimal? Latitude,
    decimal? Longitude) : ICommand<bool>;


