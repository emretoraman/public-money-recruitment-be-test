using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel 
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>() 
            };

            var rental = _rentals[rentalId];

            //Get only related bookings
            var bookings = _bookings.Values
                .Where(b => b.RentalId == rentalId
                    && b.Start.Date < start.Date.AddDays(nights + rental.PreparationTimeInDays)
                    && b.Start.Date.AddDays(b.Nights + rental.PreparationTimeInDays) > start.Date)
                .ToList();

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

                foreach (var booking in bookings)
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

        private static int GetUnit(Dictionary<int, int> bookingUnits, bool[] isUnitBlocked, bool[] isUnitBlockedTomorrow, BookingViewModel booking, RentalViewModel rental, DateTime date)
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
