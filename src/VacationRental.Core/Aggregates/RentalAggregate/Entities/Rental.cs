using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Events;
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
            _bookings = new List<Booking>();
        }

        public void AddBooking(Booking newBooking)
        {
            for (var i = 0; i < newBooking.Nights; i++)
            {
                var date = newBooking.Start.Date.AddDays(i);
                var count = 0;
                foreach (var booking in _bookings!)
                {
                    if (booking.Start.Date <= date
                        && booking.Start.Date.AddDays(booking.Nights + PreparationTimeInDays) > date)
                    {
                        count++;
                    }
                }
                if (count >= Units)
                    throw new ApplicationException("Not available");
            }

            _bookings!.Add(newBooking);
        }

        public void Update(int units, int preparationTimeInDays)
        {
            if (units < Units || preparationTimeInDays > PreparationTimeInDays)
            {
                var blockedUnitCounts = new Dictionary<DateTime, int>();
                foreach (var booking in _bookings!)
                {
                    var totalDays = booking.Nights + preparationTimeInDays;
                    for (var i = 0; i < totalDays; i++)
                    {
                        var date = booking.Start.Date.AddDays(i);
                        if (!blockedUnitCounts.ContainsKey(date))
                        {
                            blockedUnitCounts[date] = 1;
                        }
                        else
                        {
                            blockedUnitCounts[date]++;
                        }

                        if (blockedUnitCounts[date] > units)
                            throw new ApplicationException("Failed due to overlappings");
                    }
                }
            }

            Units = units;
            PreparationTimeInDays = preparationTimeInDays;

            RegisterDomainEvent(new RentalUpdatedEvent(this));
        }
    }
}
