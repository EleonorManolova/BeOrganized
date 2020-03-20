namespace OrganizeMe.Services.Data.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;

    public class EventService : IEventService
    {
        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly ICalendarService calendarService;

        public EventService(IDeletableEntityRepository<Event> eventRepository, ICalendarService calendarService)
        {
            this.eventRepository = eventRepository;
            this.calendarService = calendarService;
        }

        public async Task<bool> CreateAsync(EventInputViewModel eventViewModel)
        {
            Event eventFromForm = new Event
            {
                Title = eventViewModel.Title,
                Location = eventViewModel.Location,
                StartDate = eventViewModel.StartDate,
                StartTime = eventViewModel.StartTime,
                EndDate = eventViewModel.EndDate,
                EndTime = eventViewModel.EndTime,
                Description = eventViewModel.Description,
                CalendarId = eventViewModel.CalendarId,
            };

            await this.eventRepository.AddAsync(eventFromForm);
            await this.eventRepository.SaveChangesAsync();
            return true;
        }

        public ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId)
        {
            var events = this.eventRepository.All().Where(x => x.CalendarId == calendarId).To<EventCalendarViewModel>().ToList();
            return events;
        }

        public EventCreateViewModel GetEventViewModel(string googleApi, string username)
        {
            var model = new EventCreateViewModel
            {
                Input = new EventInputViewModel
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                },
                GoogleApi = googleApi,
                Calendars = this.calendarService.GetAllCalendarTitlesByUserId<CalendarEventViewModel>(username),
            };

            return model;
        }
    }
}
