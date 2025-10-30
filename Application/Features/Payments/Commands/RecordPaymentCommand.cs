using Application.Abstraction.Messaging;
using Domain.Common;
using Domain.Payments;

namespace Application.Features.Payments.Commands;

public sealed record RecordPaymentCommand(
    Guid RentalId,
    decimal Amount,
    PaymentMethod PaymentMethod) : ICommand<Result<CreatePaymentResponse>>;

public sealed record CreatePaymentResponse(
    Guid Id,
    decimal Amount,
    string PaymentMethod,
    DateTime PaidTime);
