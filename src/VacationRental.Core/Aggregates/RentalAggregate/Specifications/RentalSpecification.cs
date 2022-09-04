using Ardalis.Specification;
using VacationRental.Core.Aggregates.RentalAggregate.Entities;

namespace VacationRental.Core.Aggregates.RentalAggregate.Specifications
{
    public class RentalSpecification : Specification<Rental>, ISingleResultSpecification<Rental>
    {
        public RentalSpecification(int id, DateTime? start = null, int? nights = null)
        {
            Query.Where(r => r.Id == id);

            if (start != null && nights != null)
            {
                Query.Include(r => r.Bookings!
                    .Where(b => b.Start.Date < start.Value.Date.AddDays(nights.Value + b.Rental!.PreparationTimeInDays)
                        && b.Start.Date.AddDays(b.Nights + b.Rental!.PreparationTimeInDays) > start.Value.Date));
            }
            else
            {
                Query.Include(r => r.Bookings);
            }
        }
    }
}
