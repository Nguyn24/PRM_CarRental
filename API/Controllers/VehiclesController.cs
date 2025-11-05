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
    public sealed record VehicleResponse(
        Guid Id,
        string PlateNumber,
        string Type,
        string Status,
        string? ImageUrl,
        int BatteryLevel,
        Guid StationId,
        string StationName,
        DateTime CreatedAt);

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

        return CustomResults.Problem(result);
    }

    [HttpGet("{id:guid}", Name = "GetVehicleById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetVehicleByIdQuery(id), ct);

        if (result.IsSuccess)
        {
            var dto = WithAbsoluteImageUrl(result.Value);
            var response = ToResponse(dto);
            return Results.Ok(ApiResult<VehicleResponse>.Success(response));
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
            // Ensure absolute ImageUrl first
            var absoluteDtos = page.Items.Select(WithAbsoluteImageUrl).ToList();
            // Convert enums to strings
            var responseItems = absoluteDtos.Select(ToResponse).ToList();

            var responsePage = new Page<VehicleResponse>(responseItems, page.TotalCount)
            {
                PageNumber = page.PageNumber,
                TotalPages = page.TotalPages
            };

            return Results.Ok(ApiResult<Page<VehicleResponse>>.Success(responsePage));
        }

        return CustomResults.Problem(result);
    }

    public sealed record UpdateBatteryRequest(int BatteryLevel);

    [HttpPatch("{id:guid}/battery")]
    public async Task<IResult> UpdateBattery(
        [FromRoute] Guid id,
        [FromBody] UpdateBatteryRequest request,
        CancellationToken ct)
    {
        var command = new UpdateVehicleBatteryCommand(id, request.BatteryLevel);
        var result = await sender.Send(command, ct);

        if (result.IsSuccess)
        {
            var dto = WithAbsoluteImageUrl(result.Value);
            var response = ToResponse(dto);
            return Results.Ok(ApiResult<VehicleResponse>.Success(response));
        }

        return CustomResults.Problem(result);
    }

    public sealed record ChangeStatusRequest(Domain.Vehicles.VehicleStatus Status);

    [HttpPatch("{id:guid}/status")]
    public async Task<IResult> ChangeStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken ct)
    {
        var command = new ChangeVehicleStatusCommand(id, request.Status);
        var result = await sender.Send(command, ct);

        if (result.IsSuccess)
        {
            var dto = WithAbsoluteImageUrl(result.Value);
            var response = ToResponse(dto);
            return Results.Ok(ApiResult<VehicleResponse>.Success(response));
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

    private VehicleResponse ToResponse(VehicleDto dto)
        => new(
            dto.Id,
            dto.PlateNumber,
            dto.Type.ToString(),
            dto.Status,
            dto.ImageUrl,
            dto.BatteryLevel,
            dto.StationId,
            dto.StationName,
            dto.CreatedAt);
}
