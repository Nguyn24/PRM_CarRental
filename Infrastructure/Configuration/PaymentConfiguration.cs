using Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RentalId).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(10, 2).IsRequired();

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PaidTime).IsRequired();

        builder.HasOne(x => x.Rental)
            .WithMany(r => r.Payments)
            .HasForeignKey(x => x.RentalId);
    }
}


