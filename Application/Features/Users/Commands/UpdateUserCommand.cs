using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FullName = null,
    string? AvatarUrl = null,
    string? DriverLicenseNumber = null,
    string? IDCardNumber = null) : ICommand<Result<UserDto>>;
