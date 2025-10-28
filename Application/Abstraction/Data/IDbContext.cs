using Domain.Payments;
using Domain.Rentals;
using Domain.Stations;
using Domain.Users;
using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstraction.Data;

public interface IDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<Station> Stations { get; set; }
    DbSet<Vehicle> Vehicles { get; set; }
    DbSet<VehicleHistory> VehicleHistories { get; set; }
    DbSet<Rental> Rentals { get; set; }
    DbSet<Payment> Payments { get; set; }
   

    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}