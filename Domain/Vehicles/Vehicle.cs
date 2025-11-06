using Domain.Stations;

namespace Domain.Vehicles;

public class Vehicle
{
    public Guid Id { get; set; }
    public Guid StationId { get; set; }
    public string PlateNumber { get; set; } = null!;
    public VehicleType Type { get; set; }
    public int BatteryLevel { get; set; }
    public VehicleStatus Status { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; }

    public Station Station { get; set; } = null!;
    public ICollection<VehicleHistory> History { get; set; } = new List<VehicleHistory>();
    public ICollection<Rentals.Rental> Rentals { get; set; } = new List<Rentals.Rental>();
}


