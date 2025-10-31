using Application.Features.Rentals.Commands;
using Application.Features.Rentals.Queries;
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
}

