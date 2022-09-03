using Ardalis.Specification;
using VacationRental.Core.Aggregates.BookingAggregate.Entities;

namespace VacationRental.Core.Aggregates.BookingAggregate.Specifications
{
    public class BookingsSpecification : Specification<Booking>
    {
        public BookingsSpecification(int? rentalId, DateTime? start, int? nights, int? preparationTimeInDays)
        {
            if (rentalId != null)
            {
                Query.Where(b => b.RentalId == rentalId);
            }

            if (start != null && nights != null && preparationTimeInDays != null)
            {
                Query.Where(b => b.Start.Date < start.Value.Date.AddDays(nights.Value + preparationTimeInDays.Value)
                    && b.Start.Date.AddDays(b.Nights + preparationTimeInDays.Value) > start.Value.Date);
            }
        }
    }
}
