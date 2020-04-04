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

        Task<EventCreateViewModel> GetCreateViewModelAsync(string username);

        Task<ICollection<EventCalendarViewModel>> GetAllByCalendarIdAsync(string calendarId);

        Task<EventEditViewModel> GetEditViewModelByIdAsync(string eventId, string username);

        Task<bool> UpdateAsync(EventEditViewModel model, string eventId);

        Task<bool> DeleteAsync(string id);

        Task<ICollection<Event>> GetAllAsync(string username);
    }
}
