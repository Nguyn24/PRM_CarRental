using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Stations.Commands;

public sealed class UpdateStationCommandHandler : ICommandHandler<UpdateStationCommand, bool>
{
    private readonly IDbContext _dbContext;

    public UpdateStationCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(UpdateStationCommand request, CancellationToken cancellationToken)
    {
        var station = await _dbContext.Stations
            .FirstOrDefaultAsync(s => s.Id == request.StationId && !s.IsDeleted, cancellationToken);

        if (station is null)
            return Result.Failure<bool>(Error.NotFound("Station.NotFound", $"Station with ID {request.StationId} not found"));

        if (request.Name is not null) station.Name = request.Name;
        if (request.Address is not null) station.Address = request.Address;
        if (request.Latitude.HasValue) station.Latitude = request.Latitude.Value;
        if (request.Longitude.HasValue) station.Longitude = request.Longitude.Value;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}


