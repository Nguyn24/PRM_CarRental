using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;

namespace Application.Features.Users.Queries;

public sealed record GetAllUsersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "fullName",
    SortOrder? SortOrder = Application.Abstraction.Query.SortOrder.Asc) 
    : IQuery<Page<UserDto>>, IPageableQuery, ISortableQuery;
