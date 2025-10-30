using Application.Abstraction.Data;
using Application.Abstraction.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Queries;

public sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IDbContext _dbContext;

    public GetPaymentByIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentDto>(
                new Error("Payment.NotFound", $"Payment with ID {request.PaymentId} not found"));

        var paymentDto = new PaymentDto(
            payment.Id,
            payment.RentalId,
            payment.Amount,
            payment.PaymentMethod,
            payment.PaidTime);

        return Result.Success(paymentDto);
    }
}
