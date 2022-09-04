using MediatR;
using VacationRental.SharedKernel.Abstractions;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.SharedKernel.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAndClearEvents(List<EntityBase> entities)
        {
            foreach (var entity in entities)
            {
                var domainEvents = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false);
                }
            }
        }
    }
}
