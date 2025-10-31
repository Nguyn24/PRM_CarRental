using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;
using Domain.Vehicles;

namespace Application.Features.Vehicles.Queries;

public sealed record GetAllVehiclesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    VehicleStatus? Status = null,
    VehicleType? Type = null,
    string? SortBy = "plate",
    SortOrder? SortOrder = Application.Abstraction.Query.SortOrder.Asc
) : IQuery<Page<VehicleDto>>, IPageableQuery, ISortableQuery;

