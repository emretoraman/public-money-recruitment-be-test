using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Specifications;
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
        public async Task<ResourceIdViewModel> Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nights must be positive");

            var specification = new RentalSpecification(model.RentalId, model.Start, model.Nights);
            var rental = await _rentalRepository.SingleOrDefaultAsync(specification);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            var booking = new Booking(model.Start.Date, model.Nights);
            rental!.AddBooking(booking);
            await _rentalRepository.SaveChangesAsync();

            return _mapper.Map<ResourceIdViewModel>(booking);
        }
    }
}
