using Application.Abstraction.Data;
using Domain.Common;
using Domain.Payments;
using Domain.Rentals;
using Domain.Stations;
using Domain.Users;
using Domain.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
	DbContextOptions<ApplicationDbContext> options,
	IPublisher publisher)
	: DbContext(options), IDbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<Station> Stations { get; set; }
	public DbSet<Vehicle> Vehicles { get; set; }
	public DbSet<VehicleHistory> VehicleHistories { get; set; }
	public DbSet<Rental> Rentals { get; set; }
	public DbSet<Payment> Payments { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		modelBuilder.HasDefaultSchema(Schemas.Default);
	}
	
	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		int result = await base.SaveChangesAsync(cancellationToken);

		await PublishDomainEventsAsync();

		return result;
	}

	private async Task PublishDomainEventsAsync()
	{
		var domainEvents = ChangeTracker
			.Entries<Entity>()
			.Select(entry => entry.Entity)
			.SelectMany(entity =>
			{
				var events = entity.DomainEvents;
				entity.ClearDomainEvents();
				return events;
			})
			.ToList();

		foreach (var domainEvent in domainEvents)
		{
			await publisher.Publish(domainEvent);
		}
	}
}