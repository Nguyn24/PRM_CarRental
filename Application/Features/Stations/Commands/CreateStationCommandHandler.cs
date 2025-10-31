using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Stations;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Stations.Commands;

public sealed class CreateStationCommandHandler : ICommandHandler<CreateStationCommand, CreateStationResponse>
{
    private readonly IDbContext _dbContext;

    public CreateStationCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateStationResponse>> Handle(CreateStationCommand request, CancellationToken cancellationToken)
    {
        var stationExists = await _dbContext.Stations
            .AnyAsync(s => s.Name == request.Name, cancellationToken);

        if (stationExists)
            return Result.Failure<CreateStationResponse>(
                Error.Conflict("Station.NameExists", "Station with this name already exists"));

        var station = new Station
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _dbContext.Stations.Add(station);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateStationResponse(
            station.Id,
            station.Name,
            station.Address));
    }
}
