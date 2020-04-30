namespace BeOrganized.Services.Data.Calendar
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Administration.Calendars;

    public interface ICalendarService
    {
        ICollection<T> GetAllCalendarTitlesByUserName<T>(string username);

        string GetCalendarId(string username, string calendarTitle);

        int GetDefaultCalendarColorId(string username);

        string GetUserNameByCalendarId(string calendarId);

        ICollection<Calendar> GetAll();

        CalendarChangeModel GetCreateViewModel();

        Task<bool> CreateFromAdminAsync(CalendarViewModel calendarModel);

        CalendarChangeModel GetEditChangeViewModelById(string calendarId);

        Calendar MapCalendarViewModelToCalendar(CalendarViewModel model, string calendarId);

        Task<bool> UpdateAsync(Calendar model, string calendarId);

        ICollection<CalendarDetailsModel> GetDetailsViewModels();

        Task<Calendar> GetByIdAsync(string calendarId);

        Task<bool> DeleteAsync(string calendarId);
    }
}
