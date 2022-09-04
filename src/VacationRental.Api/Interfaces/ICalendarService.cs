using VacationRental.Api.Models;

namespace VacationRental.Api.Interfaces
{
    public interface ICalendarService
    {
        Task<CalendarViewModel> GetCalendar(int rentalId, DateTime start, int nights);
    }
}
