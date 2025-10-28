using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StationId)
            .IsRequired();

        builder.Property(x => x.PlateNumber)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.PlateNumber).IsUnique();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.BatteryLevel)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Station)
            .WithMany(s => s.Vehicles)
            .HasForeignKey(x => x.StationId);
    }
}


