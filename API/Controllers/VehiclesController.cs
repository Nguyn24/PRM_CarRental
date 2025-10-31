using Application.Features.Vehicles.Commands;
using Application.Features.Vehicles.Queries;
using Application.Abstraction.Query;
using API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Add([FromBody] AddVehicleCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => Url.RouteUrl("GetVehicleById", new { id = x.Id })!);
    }

    [HttpGet("{id:guid}", Name = "GetVehicleById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetVehicleByIdQuery(id), ct);
        return result.MatchOk();
    }

    [HttpGet]
    public async Task<IResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Domain.Vehicles.VehicleStatus? status = null,
        [FromQuery] Domain.Vehicles.VehicleType? type = null,
        [FromQuery] string? sortBy = "plate",
        [FromQuery] SortOrder? sortOrder = SortOrder.Asc,
        CancellationToken ct = default)
    {
        var query = new GetAllVehiclesQuery(pageNumber, pageSize, status, type, sortBy, sortOrder);
        var result = await sender.Send(query, ct);
        return result.MatchOk();
    }
}
