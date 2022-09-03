using MediatR;

namespace VacationRental.SharedKernel.Abstractions
{
    public abstract class DomainEventBase : INotification
    {
        public DateTime DateOccurred { get; private set; } = DateTime.UtcNow;
    }
}
