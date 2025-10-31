using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Record([FromBody] RecordPaymentCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.MatchCreated(x => Url.RouteUrl("GetPaymentById", new { id = x.Id })!);
    }

    [HttpGet("{id:guid}", Name = "GetPaymentById")]
    public async Task<IResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(id), ct);
        return result.MatchOk();
    }
}

