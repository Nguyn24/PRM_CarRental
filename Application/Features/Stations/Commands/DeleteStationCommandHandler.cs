using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Stations.Commands;

public sealed class DeleteStationCommandHandler : ICommandHandler<DeleteStationCommand, bool>
{
    private readonly IDbContext _dbContext;

    public DeleteStationCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(DeleteStationCommand request, CancellationToken cancellationToken)
    {
        var station = await _dbContext.Stations
            .Include(s => s.Vehicles)
            .FirstOrDefaultAsync(s => s.Id == request.StationId, cancellationToken);

        if (station is null)
            return Result.Failure<bool>(Error.NotFound("Station.NotFound", $"Station with ID {request.StationId} not found"));

        station.IsDeleted = true;

        // Optionally soft-delete vehicles in station as well to keep hidden in lists
        foreach (var v in station.Vehicles)
        {
            v.IsDeleted = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}


