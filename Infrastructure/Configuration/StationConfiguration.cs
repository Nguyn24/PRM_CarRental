using Domain.Stations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasPrecision(10, 6)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasPrecision(10, 6)
            .IsRequired();

		builder.Property(x => x.IsDeleted)
			.IsRequired()
			.HasDefaultValue(false);
    }
}


