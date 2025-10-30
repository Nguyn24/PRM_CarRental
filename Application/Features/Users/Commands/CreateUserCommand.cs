using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Users;

namespace Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    string? DriverLicenseNumber = null,
    string? IDCardNumber = null) : ICommand<Result<CreateUserResponse>>;

public sealed record CreateUserResponse(
    Guid Id,
    string Email,
    string FullName,
    UserRole Role);
