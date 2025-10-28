using Domain.Rentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleId).IsRequired();
        builder.Property(x => x.RenterId).IsRequired();
        builder.Property(x => x.StationId).IsRequired();
        builder.Property(x => x.StaffId).IsRequired();

        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime);

        builder.Property(x => x.TotalCost)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.Rentals)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Renter)
            .WithMany()
            .HasForeignKey(x => x.RenterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Staff)
            .WithMany()
            .HasForeignKey(x => x.StaffId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId);
    }
}


