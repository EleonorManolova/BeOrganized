namespace OrganizeMe.Services.Data.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Models;
    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<bool> CreateAsync(EventViewModel eventViewModel);

        Task<Event> GetByIdAsync(string id);

        EventCreateViewModel GetCreateViewModel(string username);

        ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId);

        EventEditViewModel GetEditViewModelById(string eventId, string username);

        Task<bool> UpdateAsync(EventEditViewModel model, string eventId);

        Task<bool> DeleteAsync(string id);

        ICollection<Event> GetAll(string username);
    }
}
