using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Specifications;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRepository<Rental> _rentalRepository;
        private readonly IMapper _mapper;

        public RentalsController(IRepository<Rental> rentalRepository, IMapper mapper)
        {
            _rentalRepository = rentalRepository;
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

            var specification = new RentalSpecification(rentalId);
            var rental = await _rentalRepository.SingleOrDefaultAsync(specification);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            rental.Update(model.Units, model.PreparationTimeInDays);
            await _rentalRepository.SaveChangesAsync();

            return _mapper.Map<ResourceIdViewModel>(rental);
        }
    }
}
