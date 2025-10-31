using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;

namespace Application.Features.Vehicles.Queries;

public sealed record GetVehiclesByStationQuery(
    Guid StationId,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<Page<VehicleDto>>, IPageableQuery;

