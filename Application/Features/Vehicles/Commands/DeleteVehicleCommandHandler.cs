using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Vehicles.Commands;

public sealed class DeleteVehicleCommandHandler : ICommandHandler<DeleteVehicleCommand, bool>
{
    private readonly IDbContext _dbContext;

    public DeleteVehicleCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

        if (vehicle is null)
            return Result.Failure<bool>(Error.NotFound("Vehicle.NotFound", $"Vehicle with ID {request.VehicleId} not found"));

        vehicle.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}


