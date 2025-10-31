using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;

namespace Application.Features.Stations.Queries;

public sealed record GetAllStationsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "name",
    SortOrder? SortOrder = Application.Abstraction.Query.SortOrder.Asc
) : IQuery<Page<StationDto>>, IPageableQuery, ISortableQuery;

