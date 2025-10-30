using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) : ICommand<Result<bool>>;
