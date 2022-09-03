using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;

namespace VacationRental.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<Rental, RentalViewModel>();
            CreateMap<Rental, ResourceIdViewModel>();

            CreateMap<Booking, BookingViewModel>();
            CreateMap<Booking, ResourceIdViewModel>();
        }
    }
}
