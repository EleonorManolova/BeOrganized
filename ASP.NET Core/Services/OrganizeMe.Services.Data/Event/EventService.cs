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

        public async Task<bool> CreateAsync(EventViewModel eventViewModel)
        {
            Event eventFromForm = new Event
            {
                Title = eventViewModel.Title,
                Location = eventViewModel.Location,
                StartDateTime = eventViewModel.StartDateTime,
                EndDateTime = eventViewModel.EndDateTime,
                Description = eventViewModel.Description,
                CalendarId = eventViewModel.CalendarId,
                Coordinates = eventViewModel.Coordinates.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToString(),
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

        public EventEditViewModel GetEventById(string eventId, string username)
        {
            var eventFromDb = this.eventRepository.All().Where(x => x.Id == eventId).To<EventViewModel>().First();
            var eventResult = new EventEditViewModel
            {
                Output = eventFromDb,
                Calendars = this.GetAllCalendarTitlesByUsername<CalendarEventViewModel>(username),
            };
            return eventResult;
        }

        public ICollection<T> GetAllCalendarTitlesByUsername<T>(string username)
        {
            return this.calendarService.GetAllCalendarTitlesByUserId<T>(username);
        }

        public EventCreateViewModel GetEventViewModel(string username)
        {
            var model = new EventCreateViewModel
            {
                Input = new EventViewModel
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                },
                Calendars = this.GetAllCalendarTitlesByUsername<CalendarEventViewModel>(username),
            };

            return model;
        }

        public async Task UpdateEvent(EventEditViewModel model, string eventId)
        {
            var eventNew = new Event
            {
                Id = eventId,
                Title = model.Output.Title,
                Location = model.Output.Location,
                StartDateTime = model.Output.StartDateTime,
                EndDateTime = model.Output.EndDateTime,
                Description = model.Output.Description,
                CalendarId = model.Output.CalendarId,
                Coordinates = model.Output.Coordinates.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToString(),
            };

            this.eventRepository.Update(eventNew);
            await this.eventRepository.SaveChangesAsync();
        }
    }
}
