using Application.Features.Stations.Commands;
using Application.Features.Stations.Queries;
using Application.Features.Vehicles.Queries;
using API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateStationCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => $"/api/stations/{x.Id}");
    }

    [HttpGet("{id:guid}", Name = "GetStationById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetStationByIdQuery(id), ct);
        return result.MatchOk();
    }

    [HttpGet]
    public async Task<IResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "name",
        [FromQuery] Application.Abstraction.Query.SortOrder? sortOrder = Application.Abstraction.Query.SortOrder.Asc,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAllStationsQuery(pageNumber, pageSize, sortBy, sortOrder), ct);
        return result.MatchOk();
    }

    [HttpGet("{id:guid}/vehicles")]
    public async Task<IResult> GetVehiclesInStation([FromRoute] Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetVehiclesByStationQuery(id, pageNumber, pageSize), ct);
        return result.MatchOk();
    }

    public sealed record UpdateStationRequest(
        string? Name,
        string? Address,
        decimal? Latitude,
        decimal? Longitude);

    [HttpPut("{id:guid}")]
    public async Task<IResult> Update([FromRoute] Guid id, [FromBody] UpdateStationRequest body, CancellationToken ct)
    {
        var command = new UpdateStationCommand(id, body.Name, body.Address, body.Latitude, body.Longitude);
        var result = await sender.Send(command, ct);
        return result.MatchOk();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStationCommand(id), ct);
        return result.MatchOk();
    }
}
