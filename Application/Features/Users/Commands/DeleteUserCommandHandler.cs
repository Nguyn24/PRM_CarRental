using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly IDbContext _dbContext;

    public DeleteUserCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<bool>(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found"));

        user.Status = UserStatus.Inactive;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
