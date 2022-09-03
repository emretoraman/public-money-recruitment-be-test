using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.BookingAggregate.Specifications;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;

        public BookingsController(
            IRepository<Rental> rentalRepository,
            IRepository<Booking> bookingRepository,
            IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public async Task<BookingViewModel> Get(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new ApplicationException("Booking not found");

            return _mapper.Map<BookingViewModel>(booking);
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> PostAsync(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nights must be positive");

            var rental = await _rentalRepository.GetByIdAsync(model.RentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            //Get only related bookings
            var specification = new BookingsSpecification(model.RentalId, model.Start.Date, model.Nights, rental.PreparationTimeInDays);
            var bookings = await _bookingRepository.ListAsync(specification); 

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

            var newBooking = new Booking(model.RentalId, model.Start.Date, model.Nights);
            await _bookingRepository.AddAsync(newBooking);

            return _mapper.Map<ResourceIdViewModel>(newBooking);
        }
    }
}
