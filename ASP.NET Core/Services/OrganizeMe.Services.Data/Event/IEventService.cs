namespace OrganizeMe.Services.Data.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<bool> CreateAsync(EventInputViewModel eventViewModel);

        EventCreateViewModel GetEventViewModel(string googleApi, string username);

        ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId);
    }
}
