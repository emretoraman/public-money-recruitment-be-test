using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            if (!_bookings.ContainsKey(bookingId))
                throw new ApplicationException("Booking not found");

            return _bookings[bookingId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nights must be positive");
            if (!_rentals.ContainsKey(model.RentalId))
                throw new ApplicationException("Rental not found");

            var rental = _rentals[model.RentalId];

            //Get only related bookings
            var bookings = _bookings.Values
                .Where(b => b.RentalId == model.RentalId
                    && b.Start.Date < model.Start.Date.AddDays(model.Nights + rental.PreparationTimeInDays)
                    && b.Start.Date.AddDays(b.Nights + rental.PreparationTimeInDays) > model.Start.Date)
                .ToList();

            for (var i = 0; i < model.Nights; i++)
            {
                var date = model.Start.Date.AddDays(i);
                var count = 0;
                foreach (var booking in bookings)
                {
                    if (booking.Start.Date <= date
                        && booking.Start.Date.AddDays(booking.Nights + rental.PreparationTimeInDays) > date)
                    {
                        count++;
                    }
                }
                if (count >= rental.Units)
                    throw new ApplicationException("Not available");
            }

            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date
            });

            return key;
        }
    }
}
