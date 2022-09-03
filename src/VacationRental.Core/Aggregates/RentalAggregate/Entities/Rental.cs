using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.SharedKernel.Abstractions;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Core.Aggregates.RentalAggregate.Entities
{
    public class Rental : EntityBase, IAggregateRoot
    {
        //Used by EF
        private Rental()
        {
        }

        public int Id { get; private set; }

        public int Units { get; private set; }
        public int PreparationTimeInDays { get; private set; }

        private readonly List<Booking>? _bookings = null;
        public IReadOnlyCollection<Booking>? Bookings => _bookings?.AsReadOnly();

        public Rental(int units, int preparationTimeInDays)
        {
            Units = units;
            PreparationTimeInDays = preparationTimeInDays;
        }

        public void Update(int units, int preparationTimeInDays)
        {
            Units = units;
            PreparationTimeInDays = preparationTimeInDays;
        }
    }
}
