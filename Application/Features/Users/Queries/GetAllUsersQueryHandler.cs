using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Application.Abstraction.Query;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Page<UserDto>>
{
    private readonly IDbContext _dbContext;

    public GetAllUsersQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Page<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsQueryable();

        query = request.SortBy?.ToLower() switch
        {
            "email" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(u => u.Email)
                : query.OrderByDescending(u => u.Email),
            "created" => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(u => u.CreatedAt)
                : query.OrderByDescending(u => u.CreatedAt),
            _ => request.SortOrder == SortOrder.Asc
                ? query.OrderBy(u => u.FullName)
                : query.OrderByDescending(u => u.FullName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Email,
                u.Role.ToString(),
                u.Status.ToString(),
                u.CreatedAt,
                u.IsVerified,
                u.AvatarUrl))
            .ToListAsync(cancellationToken);

        var page = new Page<UserDto>(
            users,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(page);
    }
}
