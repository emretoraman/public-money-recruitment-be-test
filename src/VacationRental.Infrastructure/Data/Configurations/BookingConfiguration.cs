using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;

namespace VacationRental.Infrastructure.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.RentalId);
            builder.HasOne(b => b.Rental)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.Start);
            builder.Property(b => b.Nights);
        }
    }
}
