using Application.Abstraction.Messaging;
using Domain.Common;

namespace Application.Features.Rentals.Commands;

public sealed record CompleteRentalCommand(
    Guid RentalId,
    Guid EndStationId,
    int FinalBatteryLevel,
    string? Notes = null) : ICommand<CompleteRentalResponse>;

public sealed record CompleteRentalResponse(
    Guid RentalId,
    DateTime EndTime,
    decimal TotalCost,
    string Status);

