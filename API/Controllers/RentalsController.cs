using Application.Features.Rentals.Commands;
using Application.Features.Rentals.Queries;
using Domain.Rentals;
using API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController(ISender sender) : ControllerBase
{
    [HttpPost("start")]
    public async Task<IResult> Start([FromBody] StartRentalCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => Url.RouteUrl("GetRentalById", new { id = x.RentalId })!);
    }

    [HttpGet("{id:guid}", Name = "GetRentalById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetRentalByIdQuery(id), ct);
        return result.MatchOk();
    }

    [HttpGet]
    public async Task<IResult> GetUserRentals(
        [FromQuery] Guid? userId,
        [FromQuery] Domain.Rentals.RentalStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetUserRentalsQuery(userId, status, pageNumber, pageSize);
        var result = await sender.Send(query, ct);
        return result.MatchOk();
    }

    public sealed record CompleteRentalRequest(
        Guid EndStationId,
        int FinalBatteryLevel,
        string? Notes);

    [HttpPost("{id:guid}/complete")]
    public async Task<IResult> Complete(
        [FromRoute] Guid id,
        [FromBody] CompleteRentalRequest request,
        CancellationToken ct)
    {
        var command = new CompleteRentalCommand(
            id,
            request.EndStationId,
            request.FinalBatteryLevel,
            request.Notes);
        
        var result = await sender.Send(command, ct);
        return result.MatchOk();
    }
}

