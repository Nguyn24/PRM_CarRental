using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Stations.Commands;

public sealed record CreateStationCommand(
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude) : ICommand<CreateStationResponse>;

public sealed record CreateStationResponse(
    Guid Id,
    string Name,
    string Address);
