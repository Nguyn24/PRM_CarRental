using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Stations.Queries;

public sealed record GetStationByIdQuery(Guid StationId) : IQuery<StationDto>;

public sealed record StationDto(
    Guid Id,
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    int AvailableVehicles
);

