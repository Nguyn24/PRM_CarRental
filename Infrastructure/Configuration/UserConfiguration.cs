using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Users;

namespace Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.IsRequired();

		builder.Property(x => x.FullName)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(x => x.Email)
			.HasMaxLength(255)
			.IsRequired();
		builder.HasIndex(x => x.Email).IsUnique();

		builder.Property(x => x.PasswordHash)
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.Role)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(x => x.Status)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(x => x.DriverLicenseNumber)
			.HasMaxLength(50);

		builder.Property(x => x.IDCardNumber)
			.HasMaxLength(50);
		
		builder.Property(x => x.CreatedAt)
			.IsRequired();
		
		builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);

		builder.Property(x => x.AvatarUrl).HasMaxLength(1000);

		builder.Property(x => x.IsDeleted)
			.IsRequired()
			.HasDefaultValue(false);
	}
}