using VacationRental.Api.Interfaces;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Specifications;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Api.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IRepository<Rental> _rentalRepository;

        public CalendarService(IRepository<Rental> rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        public async Task<CalendarViewModel> GetCalendar(int rentalId, DateTime start, int nights)
        {
            var specification = new RentalSpecification(rentalId, start, nights);
            var rental = await _rentalRepository.SingleOrDefaultAsync(specification);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            var bookingUnits = new Dictionary<int, int>();
            var isUnitBlockedTomorrow = new bool[rental.Units + 1];

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                var isUnitBlocked = isUnitBlockedTomorrow;
                isUnitBlockedTomorrow = new bool[rental.Units + 1];

                foreach (var booking in rental.Bookings!)
                {
                    if (booking.Start.Date <= date.Date)
                    {
                        if (booking.Start.Date.AddDays(booking.Nights) > date.Date)
                        {
                            var unit = GetUnit(bookingUnits, isUnitBlocked, isUnitBlockedTomorrow, booking, rental, date.Date);
                            date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = unit });
                        }
                        else if (booking.Start.Date.AddDays(booking.Nights + rental.PreparationTimeInDays) > date.Date)
                        {
                            var unit = GetUnit(bookingUnits, isUnitBlocked, isUnitBlockedTomorrow, booking, rental, date.Date);
                            date.PreparationTimes.Add(new CalendarPreparationTimeViewModel { Unit = unit });
                        }
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }

        private static int GetUnit(Dictionary<int, int> bookingUnits, bool[] isUnitBlocked, bool[] isUnitBlockedTomorrow, Booking booking, Rental rental, DateTime date)
        {
            var unit = 0;
            if (bookingUnits.ContainsKey(booking.Id)) //Already assigned unit
            {
                unit = bookingUnits[booking.Id];
            }
            else
            {
                for (var i = 1; i <= rental.Units; i++)
                {
                    if (!isUnitBlocked[i])
                    {
                        unit = i;
                        bookingUnits[booking.Id] = unit;
                        break;
                    }
                }
            }

            isUnitBlocked[unit] = true;
            isUnitBlockedTomorrow[unit] = booking.Start.Date.AddDays(booking.Nights + rental.PreparationTimeInDays) > date.Date.AddDays(1);

            return unit;
        }
    }
}
