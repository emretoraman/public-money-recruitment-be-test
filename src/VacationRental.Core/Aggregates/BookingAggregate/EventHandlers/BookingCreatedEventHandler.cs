using MediatR;
using VacationRental.Core.Aggregates.BookingAggregate.Events;

namespace VacationRental.Core.Aggregates.BookingAggregate.EventHandlers
{
    public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
    {
        public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
        }
    }
}
