namespace OrganizeMe.Services.Data.Calendar
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;
    using OrganizeMe.Common;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Calendar;

    public class CalendarService : ICalendarService
    {
        private readonly IDeletableEntityRepository<Calendar> calendarRepository;

        public CalendarService(IDeletableEntityRepository<Calendar> calendarRepository)
        {
            this.calendarRepository = calendarRepository;
        }

        public ICollection<T> GetAllCalendarTitlesByUserId<T>(string username)
        {
            var result = this.calendarRepository.All().Include(x => x.User).Where(x => x.User.UserName == username).To<T>().ToList();
            return result;
        }

        public CalendarIndexViewModel GetDefaultCalendarIndexViewModel(string username)
        {
            var events = this.calendarRepository
                .All()
                .Include(x => x.User)
                .Where(x => x.User.UserName == username && x.Title == GlobalConstants.DefaultCalendarTitle)
                .Select(x => x.Events)
                // .To<EventCalendarViewModel>()
                .ToList();

            var habits = this.calendarRepository
            .All()
            .Where(x => x.User.UserName == username && x.Title == GlobalConstants.DefaultCalendarTitle)
            .Select(x => x.Habits)
            .To<HabitCalendarViewModel>()
            .ToList();

            var model = new CalendarIndexViewModel
            {
                // Events = events,
                Habits = habits,
            };

            return model;
        }
    }
}
