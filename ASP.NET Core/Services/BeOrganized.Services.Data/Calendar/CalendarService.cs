namespace BeOrganized.Services.Data.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BeOrganized.Common;
    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class CalendarService : ICalendarService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IDeletableEntityRepository<Calendar> calendarRepository;

        public CalendarService(IDeletableEntityRepository<Calendar> calendarRepository)
        {
            this.calendarRepository = calendarRepository;
        }

        public ICollection<T> GetAllCalendarTitlesByUserId<T>(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = this.calendarRepository
                .All()
                .Include(x => x.User)
                .Where(x => x.User.UserName == username)
                .To<T>()
                .ToList();
            return result;
        }

        public int GetDefaultCalendarColorId(string username)
        {
            var result = this.calendarRepository
                .All()
                .Select(x => new
                {
                    x.DefaultCalendarColorId,
                    x.User.UserName,
                })
                .Where(x => x.UserName == username)
                .Select(x => x.DefaultCalendarColorId)
                .First();
            return result;
        }

        public string GetDefaultCalendarId(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = this.calendarRepository
                .All()
                .Select(x => new
                {
                    x.Id,
                    x.User.UserName,
                    x.Title,
                })
                .Where(x => x.UserName == username && x.Title == GlobalConstants.DefaultCalendarTitle)
                .Select(x => x.Id)
                .First();
            return result;
        }
    }
}
