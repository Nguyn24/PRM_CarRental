using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;
using Domain.Rentals;

namespace Application.Features.Rentals.Queries;

public sealed record GetUserRentalsQuery(
    Guid? UserId = null,
    RentalStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<Page<RentalDto>>, IPageableQuery;

