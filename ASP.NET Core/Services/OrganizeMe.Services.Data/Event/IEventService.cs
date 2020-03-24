namespace OrganizeMe.Services.Data.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<bool> CreateAsync(EventViewModel eventViewModel);

        EventCreateViewModel GetEventViewModel(string username);

        ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId);

        EventEditViewModel GetEventById(string eventId, string username);

        Task UpdateEvent(EventEditViewModel model, string eventId);

        ICollection<T> GetAllCalendarTitlesByUsername<T>(string username);
    }
}
