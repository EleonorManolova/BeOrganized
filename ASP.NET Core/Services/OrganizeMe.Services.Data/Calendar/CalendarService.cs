namespace OrganizeMe.Services.Data.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using OrganizeMe.Common;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class CalendarService : ICalendarService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IDeletableEntityRepository<Calendar> calendarRepository;

        public CalendarService(IDeletableEntityRepository<Calendar> calendarRepository)
        {
            this.calendarRepository = calendarRepository;
        }

        public async Task<ICollection<T>> GetAllCalendarTitlesByUserIdAsync<T>(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = await this.calendarRepository.All().Include(x => x.User).Where(x => x.User.UserName == username).To<T>().ToListAsync();
            return result;
        }

        public async Task<int> GetDefaultCalendarColorIdAsync(string username)
        {
            var result = await this.calendarRepository.All().Include(x => x.User).Where(x => x.User.UserName == username).Select(x => x.DefaultCalendarColorId).FirstAsync();
            return result;
        }

        public async Task<string> GetDefaultCalendarIdAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = await this.calendarRepository
                .All()
                .Include(x => x.User)
                .Where(x => x.User.UserName == username && x.Title == GlobalConstants.DefaultCalendarTitle)
                .Select(x => x.Id)
                .FirstAsync();
            return result;
        }
    }
}
