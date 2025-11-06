using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Abstraction.Query;
using API.Extensions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => Url.RouteUrl("GetUserById", new { id = x.Id })!);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), ct);
        return result.MatchOk();
    }

    [HttpGet]
    public async Task<IResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "fullName",
        [FromQuery] SortOrder? sortOrder = SortOrder.Asc,
        [FromQuery] UserRole? role = null,
        CancellationToken ct = default)
    {
        var query = new GetAllUsersQuery(pageNumber, pageSize, sortBy, sortOrder, role);
        var result = await sender.Send(query, ct);
        return result.MatchOk();
    }

    public sealed record UpdateUserRequest(
        string? FullName,
        string? AvatarUrl,
        string? DriverLicenseNumber,
        string? IDCardNumber);

    [HttpPut("{id:guid}")]
    public async Task<IResult> Update([FromRoute] Guid id, [FromBody] UpdateUserRequest body, CancellationToken ct)
    {
        var command = new UpdateUserCommand(
            UserId: id,
            FullName: body.FullName,
            AvatarUrl: body.AvatarUrl,
            DriverLicenseNumber: body.DriverLicenseNumber,
            IDCardNumber: body.IDCardNumber);

        var result = await sender.Send(command, ct);
        return result.MatchOk();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteUserCommand(id), ct);
        return result.MatchOk();
    }
}
