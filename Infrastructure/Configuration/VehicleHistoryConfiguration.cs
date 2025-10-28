using Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class VehicleHistoryConfiguration : IEntityTypeConfiguration<VehicleHistory>
{
    public void Configure(EntityTypeBuilder<VehicleHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleId)
            .IsRequired();

        builder.Property(x => x.TimeStamp)
            .IsRequired();

        builder.Property(x => x.BatteryLevel)
            .IsRequired();

        builder.Property(x => x.ConditionNote)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.History)
            .HasForeignKey(x => x.VehicleId);
    }
}


