using Ardalis.Specification.EntityFrameworkCore;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.Infrastructure.Data
{
    public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
    {
        public EfRepository(VacationRentalDbContext dbContext) : base(dbContext)
        { 
        }
    }
}
