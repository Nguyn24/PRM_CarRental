namespace Domain.Stations;

public class Station
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public ICollection<Vehicles.Vehicle> Vehicles { get; set; } = new List<Vehicles.Vehicle>();
}


