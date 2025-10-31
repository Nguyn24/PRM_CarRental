using Application.Features.Auth.Commands;
using Application.Features.Users.Commands;
using API.Extensions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    public sealed record LoginRequest(string Email, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), ct);
        return result.MatchOk();
    }

    public sealed record RegisterRequest(
        string FullName,
        string Email,
        string Password,
        string? DriverLicenseNumber,
        string? IDCardNumber);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IResult> Register([FromBody] RegisterRequest body, CancellationToken ct)
    {
        var command = new CreateUserCommand(
            FullName: body.FullName,
            Email: body.Email,
            Password: body.Password,
            Role: UserRole.Renter,
            DriverLicenseNumber: body.DriverLicenseNumber,
            IDCardNumber: body.IDCardNumber);

        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => Url.RouteUrl("GetUserById", new { id = x.Id })!);
    }
}

