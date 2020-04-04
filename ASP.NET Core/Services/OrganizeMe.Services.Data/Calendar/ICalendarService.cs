namespace OrganizeMe.Services.Data.Calendar
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Models;

    public interface ICalendarService
    {
        Task<ICollection<T>> GetAllCalendarTitlesByUserIdAsync<T>(string username);

        Task<string> GetDefaultCalendarIdAsync(string username);

        Task<int> GetDefaultCalendarColorIdAsync(string username);
    }
}
