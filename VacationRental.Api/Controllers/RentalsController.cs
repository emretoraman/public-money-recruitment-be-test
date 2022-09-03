using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public RentalsController(IDictionary<int, RentalViewModel> rentals, IDictionary<int, BookingViewModel> bookings)
        {
            _rentals = rentals;
            _bookings = bookings;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            if (model.Units < 0)
                throw new ApplicationException("Units must be positive");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays must be positive");

            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PreparationTimeInDays = model.PreparationTimeInDays
            });

            return key;
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public ResourceIdViewModel Put(int rentalId, RentalBindingModel model)
        {
            if (model.Units < 0)
                throw new ApplicationException("Units must be positive");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays must be positive");
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            var rental = _rentals[rentalId];

            if (model.Units < rental.Units || model.PreparationTimeInDays > rental.PreparationTimeInDays)
            {
                //Get only related bookings
                var bookings = _bookings.Values
                    .Where(b => b.RentalId == rentalId)
                    .ToList();

                var blockedUnitCounts = new Dictionary<DateTime, int>();
                foreach (var booking in bookings)
                {
                    var totalDays = booking.Nights + model.PreparationTimeInDays;
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

                        if (blockedUnitCounts[date] > model.Units)
                        {
                            throw new ApplicationException("Failed due to overlappings");
                        }
                    }
                }
            }

            rental.Units = model.Units;
            rental.PreparationTimeInDays = model.PreparationTimeInDays;

            return new ResourceIdViewModel { Id = rentalId };
        }
    }
}
