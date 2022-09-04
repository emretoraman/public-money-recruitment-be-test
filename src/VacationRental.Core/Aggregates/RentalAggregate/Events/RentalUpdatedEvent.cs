using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.SharedKernel.Abstractions;

namespace VacationRental.Core.Aggregates.RentalAggregate.Events
{
    public class RentalUpdatedEvent : DomainEventBase
    {
        public Rental Rental { get; private set; }

        public RentalUpdatedEvent(Rental rental)
        {
            Rental = rental;
        }
    }
}
