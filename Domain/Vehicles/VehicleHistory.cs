namespace Domain.Vehicles;

public class VehicleHistory
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime TimeStamp { get; set; }
    public int BatteryLevel { get; set; }
    public string? ConditionNote { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
}


