namespace OrganizeMe.Services.Data.Calendar
{
    using System.Collections.Generic;

    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Habits;

    public interface ICalendarService
    {
        CalendarIndexViewModel GetDefaultCalendarIndexViewModel(string username);

        ICollection<T> GetAllCalendarTitlesByUserId<T>(string username);
    }
}
