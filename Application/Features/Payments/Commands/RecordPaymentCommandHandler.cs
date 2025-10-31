using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands;

public sealed class RecordPaymentCommandHandler : ICommandHandler<RecordPaymentCommand, CreatePaymentResponse>
{
    private readonly IDbContext _dbContext;

    public RecordPaymentCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreatePaymentResponse>> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == request.RentalId, cancellationToken);

        if (rental is null)
            return Result.Failure<CreatePaymentResponse>(
                Error.NotFound("Rental.NotFound", "Rental not found"));

        if (request.Amount <= 0)
            return Result.Failure<CreatePaymentResponse>(
                Error.Validation("Payment.InvalidAmount", "Payment amount must be greater than 0"));

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            RentalId = request.RentalId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaidTime = DateTime.UtcNow
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreatePaymentResponse(
            payment.Id,
            payment.Amount,
            payment.PaymentMethod.ToString(),
            payment.PaidTime));
    }
}
