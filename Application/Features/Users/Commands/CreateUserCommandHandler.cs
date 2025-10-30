using Application.Abstraction.Authentication;
using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (userExists)
            return Result.Failure<CreateUserResponse>(
                new Error("User.EmailExists", "User with this email already exists"));

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            IsVerified = false,
            DriverLicenseNumber = request.DriverLicenseNumber,
            IDCardNumber = request.IDCardNumber
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.Role));
    }
}
