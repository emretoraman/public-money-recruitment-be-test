using VacationRental.Core.Aggregates.BookingAggregate.Events;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.SharedKernel.Abstractions;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Core.Aggregates.BookingAggregate.Entities
{
    public class Booking : EntityBase, IAggregateRoot
    {
        //Used by EF
        private Booking()
        {
        }

        public int Id { get; private set; }

        public int RentalId { get; private set; }
        public Rental? Rental { get; private set; }

        public DateTime Start { get; private set; }
        public int Nights { get; private set; }

        public Booking(int rentalId, DateTime start, int nights)
        {
            RentalId = rentalId;
            Start = start;
            Nights = nights;

            RegisterDomainEvent(new BookingCreatedEvent(this));
        }
    }
}
