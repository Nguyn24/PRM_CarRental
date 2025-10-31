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
}
