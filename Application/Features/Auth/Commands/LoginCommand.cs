using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;

public sealed record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);

