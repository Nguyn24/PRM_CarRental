using Application.Abstraction.Authentication;
using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginCommandHandler(IDbContext dbContext, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
            return Result.Failure<AuthResponse>(
                Error.NotFound("Auth.InvalidCredentials", "Invalid email or password"));

        if (user.Status != UserStatus.Active)
            return Result.Failure<AuthResponse>(
                Error.Problem("Auth.Inactive", "User is not active"));

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(
                Error.Problem("Auth.InvalidCredentials", "Invalid email or password"));

        string accessToken = _tokenProvider.Create(user);
        string refreshToken = _tokenProvider.GenerateRefreshToken();

        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        _dbContext.RefreshTokens.Add(rt);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60));

        return Result.Success(response);
    }
}

