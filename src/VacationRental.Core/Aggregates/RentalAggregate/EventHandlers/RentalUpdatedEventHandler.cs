using MediatR;
using VacationRental.Core.Aggregates.RentalAggregate.Events;

namespace VacationRental.Core.Aggregates.RentalAggregate.EventHandlers
{
    public class RentalUpdatedEventHandler : INotificationHandler<RentalUpdatedEvent>
    {
        public async Task Handle(RentalUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
        }
    }
}
