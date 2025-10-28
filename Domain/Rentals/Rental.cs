using Domain.Stations;
using Domain.Users;
using Domain.Vehicles;

namespace Domain.Rentals;

public class Rental
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid RenterId { get; set; }
    public Guid StationId { get; set; }
    public Guid StaffId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal TotalCost { get; set; }
    public RentalStatus Status { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
    public User Renter { get; set; } = null!;
    public Station Station { get; set; } = null!;
    public User Staff { get; set; } = null!;
    public ICollection<Payments.Payment> Payments { get; set; } = new List<Payments.Payment>();
}


