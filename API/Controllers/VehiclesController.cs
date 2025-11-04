using Application.Features.Vehicles.Commands;
using Application.Features.Vehicles.Queries;
using Application.Abstraction.Query;
using API.Extensions;
using API.Infrastructure;
using API.Services;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(ISender sender, IVehicleImageStorage imageStorage) : ControllerBase
{
    public sealed class CreateVehicleRequest
    {
        public string PlateNumber { get; set; } = null!;
        public Domain.Vehicles.VehicleType Type { get; set; }
        public Guid StationId { get; set; }
        public int BatteryLevel { get; set; } = 100;
        public IFormFile? Image { get; set; }
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IResult> Add([FromForm] CreateVehicleRequest request, CancellationToken ct)
    {
        string? imageUrl = null;

        if (request.Image is { Length: > 0 })
        {
            imageUrl = await imageStorage.SaveAsync(request.Image, ct);
        }

        var command = new AddVehicleCommand(
            request.PlateNumber,
            request.Type,
            request.StationId,
            request.BatteryLevel,
            imageUrl);

        var result = await sender.Send(command, ct);

        if (result.IsSuccess)
        {
            var response = result.Value with
            {
                ImageUrl = BuildAbsoluteUrl(result.Value.ImageUrl)
            };

            var location = Url.RouteUrl("GetVehicleById", new { id = response.Id })!;
            return Results.Created(location, ApiResult<CreateVehicleResponse>.Success(response));
        }

        return result.MatchCreated(x => Url.RouteUrl("GetVehicleById", new { id = x.Id })!);
    }

    [HttpGet("{id:guid}", Name = "GetVehicleById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetVehicleByIdQuery(id), ct);

        if (result.IsSuccess)
        {
            var dto = WithAbsoluteImageUrl(result.Value);
            return Results.Ok(ApiResult<VehicleDto>.Success(dto));
        }

        return CustomResults.Problem(result);
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

        if (result.IsSuccess)
        {
            var page = result.Value;
            if (page.Items is IList<VehicleDto> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    list[i] = WithAbsoluteImageUrl(list[i]);
                }
            }
            else
            {
                var rewritten = page.Items.Select(WithAbsoluteImageUrl).ToList();
                page.Items.Clear();
                foreach (var item in rewritten)
                {
                    page.Items.Add(item);
                }
            }

            return Results.Ok(ApiResult<Page<VehicleDto>>.Success(page));
        }

        return CustomResults.Problem(result);
    }

    private string? BuildAbsoluteUrl(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return relativePath;
        }

        return $"{Request.Scheme}://{Request.Host}{relativePath}";
    }

    private VehicleDto WithAbsoluteImageUrl(VehicleDto dto)
        => dto with { ImageUrl = BuildAbsoluteUrl(dto.ImageUrl) };
}
