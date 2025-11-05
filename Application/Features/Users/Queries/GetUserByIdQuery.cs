using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;

public sealed record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string PasswordHash,
    string Role,
    string Status,
    DateTime CreatedAt,
    bool IsVerified,
    string? AvatarUrl,
    string? DriverLicenseNumber,
    string? IDCardNumber);
