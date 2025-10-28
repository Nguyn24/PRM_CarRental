namespace Domain.Payments;

public class Payment
{
    public Guid Id { get; set; }
    public Guid RentalId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaidTime { get; set; }

    public Rentals.Rental Rental { get; set; } = null!;
}


