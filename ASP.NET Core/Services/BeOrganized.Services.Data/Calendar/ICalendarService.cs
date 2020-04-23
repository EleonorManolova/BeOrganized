namespace BeOrganized.Services.Data.Calendar
{
    using System.Collections.Generic;

    public interface ICalendarService
    {
        ICollection<T> GetAllCalendarTitlesByUserName<T>(string username);

        string GetDefaultCalendarId(string username);

        int GetDefaultCalendarColorId(string username);

        string GetUserNameByCalendarId(string calendarId);
    }
}
