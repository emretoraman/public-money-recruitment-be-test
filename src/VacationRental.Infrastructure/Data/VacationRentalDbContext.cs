using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.SharedKernel.Abstractions;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Infrastructure.Data
{
    public class VacationRentalDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public VacationRentalDbContext(
            DbContextOptions<VacationRentalDbContext> options,
            IDomainEventDispatcher dispatcher
        ) : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<Booking> Booking => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
