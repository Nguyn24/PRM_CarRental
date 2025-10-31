using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Payments;

namespace Application.Features.Payments.Queries;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentDto>;

public sealed record PaymentDto(
    Guid Id,
    Guid RentalId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTime PaidTime);
