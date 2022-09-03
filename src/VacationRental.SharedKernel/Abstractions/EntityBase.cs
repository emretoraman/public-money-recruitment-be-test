using System.ComponentModel.DataAnnotations.Schema;

namespace VacationRental.SharedKernel.Abstractions
{
    public abstract class EntityBase
    {
        private readonly List<DomainEventBase> _domainEvents = new();
        [NotMapped]
        public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

        protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
        internal void ClearDomainEvents() => _domainEvents.Clear();
    }
}
