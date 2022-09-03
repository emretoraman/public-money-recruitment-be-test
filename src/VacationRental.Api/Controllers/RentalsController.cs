using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.BookingAggregate.Specifications;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;

        public RentalsController(
            IRepository<Rental> rentalRepository, 
            IRepository<Booking> bookingRepository,
            IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public async Task<RentalViewModel> Get(int rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            return _mapper.Map<RentalViewModel>(rental);
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            if (model.Units < 0)
                throw new ApplicationException("Units must be positive");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays must be positive");

            var rental = new Rental(model.Units, model.PreparationTimeInDays);
            await _rentalRepository.AddAsync(rental);

            return _mapper.Map<ResourceIdViewModel>(rental);
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public async Task<ResourceIdViewModel> Put(int rentalId, RentalBindingModel model)
        {
            if (model.Units < 0)
                throw new ApplicationException("Units must be positive");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays must be positive");

            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            if (model.Units < rental.Units || model.PreparationTimeInDays > rental.PreparationTimeInDays)
            {
                //Get only related bookings
                var specification = new BookingsSpecification(rentalId, null, null, null);
                var bookings = await _bookingRepository.ListAsync(specification);

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
                            throw new ApplicationException("Failed due to overlappings");
                    }
                }
            }

            rental.Update(model.Units, model.PreparationTimeInDays);
            await _rentalRepository.SaveChangesAsync();

            return _mapper.Map<ResourceIdViewModel>(rental);
        }
    }
}
