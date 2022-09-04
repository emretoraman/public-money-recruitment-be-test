using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.SharedKernel.Abstractions;

namespace VacationRental.Core.Aggregates.BookingAggregate.Events
{
    public class BookingCreatedEvent : DomainEventBase
    {
        public Booking Booking { get; private set; }

        public BookingCreatedEvent(Booking booking)
        {
            Booking = booking;
        }
    }
}
