using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Application.Features.Users.Queries;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDto>
{
    private readonly IDbContext _dbContext;

    public UpdateUserCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<UserDto>(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found"));

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            user.AvatarUrl = request.AvatarUrl;

        if (!string.IsNullOrWhiteSpace(request.DriverLicenseNumber))
            user.DriverLicenseNumber = request.DriverLicenseNumber;

        if (!string.IsNullOrWhiteSpace(request.IDCardNumber))
            user.IDCardNumber = request.IDCardNumber;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString(),
            user.CreatedAt,
            user.IsVerified,
            user.AvatarUrl));
    }
}
