using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;

namespace VacationRental.Infrastructure.Data.Configurations
{
    public class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Units);
            builder.Property(r => r.PreparationTimeInDays);
        }
    }
}
