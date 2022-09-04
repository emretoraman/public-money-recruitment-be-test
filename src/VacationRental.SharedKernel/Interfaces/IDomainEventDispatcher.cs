using VacationRental.SharedKernel.Abstractions;

namespace VacationRental.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(List<EntityBase> entities);
    }
}
