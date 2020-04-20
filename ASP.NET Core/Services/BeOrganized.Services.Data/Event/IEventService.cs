namespace BeOrganized.Services.Data.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<bool> CreateAsync(EventViewModel eventViewModel);

        EventDetailsViewModel GetDetailsViewModelById(string id);

        EventChangeViewModel GetCreateChangeViewModel(string username);

        ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId);

        EventChangeViewModel GetEditChangeViewModelById(string eventId, string username);

        Task<bool> UpdateAsync(Event model, string eventId);

        Task<bool> DeleteAsync(string id);

        ICollection<Event> GetAllByUsername(string username);

        Task<Event> GetByIdAsync(string id);

        Event MapEventViewModelToEvent(EventViewModel eventViewModel, string eventId);

        ICollection<Event> GetAll();
    }
}
