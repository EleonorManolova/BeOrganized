namespace BeOrganized.Services.Data.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Events;
    using Nest;

    public class EventService : IEventService
    {
        private const string InvalidEventIdErrorMessage = "Event with Id: {0} does not exist.";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";
        private const string InvalidDateTimeErrorMessage = "The start day and time must be before the end day and time.";

        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly ICalendarService calendarService;
        private readonly ISearchService searchService;
        private readonly IColorService colorService;

        public EventService(IDeletableEntityRepository<Event> eventRepository, ICalendarService calendarService, ISearchService searchService, IColorService colorService)
        {
            this.eventRepository = eventRepository;
            this.calendarService = calendarService;
            this.searchService = searchService;
            this.colorService = colorService;
        }

        public async Task<bool> CreateAsync(EventViewModel eventViewModel)
        {
            if (string.IsNullOrEmpty(eventViewModel.Title) ||
                string.IsNullOrEmpty(eventViewModel.CalendarId) ||
                eventViewModel.StartDate == null ||
                eventViewModel.StartTime == null ||
                eventViewModel.EndDate == null ||
                eventViewModel.EndTime == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            if (eventViewModel.StartDateTime > eventViewModel.EndDateTime)
            {
                throw new ArgumentException(InvalidDateTimeErrorMessage);
            }

            Event eventFromForm = new Event
            {
                Title = eventViewModel.Title,
                Location = eventViewModel.Location,
                StartDateTime = eventViewModel.StartDateTime,
                EndDateTime = eventViewModel.EndDateTime,
                Description = eventViewModel.Description,
                CalendarId = eventViewModel.CalendarId,
                Coordinates = string.IsNullOrEmpty(eventViewModel.Coordinates) ?
                string.Empty :
                eventViewModel.Coordinates.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToString(),
                ColorId = eventViewModel.ColorId,
            };

            var response = await this.searchService.CreateIndexAsync<Event>(eventFromForm);

            await this.eventRepository.AddAsync(eventFromForm);
            var result = await this.eventRepository.SaveChangesAsync();

            return result > 0 && response == Result.Created;
        }

        public EventDetailsViewModel GetDetailsViewModelById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = this.eventRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(e => new EventDetailsViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    Location = e.Location,
                    Description = e.Description,
                    ColorHex = e.Color.Hex,
                    CalendarTitle = e.Calendar.Title,
                })
                .First();

            if (eventFromDb == null)
            {
                throw new ArgumentException(string.Format(InvalidEventIdErrorMessage, id));
            }

            return eventFromDb;
        }

        public async Task<Event> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = await this.eventRepository.GetByIdWithDeletedAsync(id);

            if (eventFromDb == null)
            {
                throw new ArgumentException(string.Format(InvalidEventIdErrorMessage, id));
            }

            return eventFromDb;
        }

        public ICollection<EventCalendarViewModel> GetAllByCalendarId(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var events = this.eventRepository
                .All()
                .Where(x => x.CalendarId == calendarId)
                .To<EventCalendarViewModel>()
                .ToList();
            return events;
        }

        public EventChangeViewModel GetEditChangeViewModelById(string eventId, string username)
        {
            if (string.IsNullOrEmpty(eventId) ||
               string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = this.eventRepository.All().Where(x => x.Id == eventId).To<EventViewModel>().First();
            var eventResult = new EventChangeViewModel
            {
                EventModel = eventFromDb,
                Calendars = this.GetAllCalendarTitlesByUsername<CalendarEventViewModel>(username),
                Colors = this.colorService.GetAllColors(),
            };
            return eventResult;
        }

        public EventChangeViewModel GetCreateChangeViewModel(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var model = new EventChangeViewModel
            {
                EventModel = new EventViewModel
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    ColorId = this.calendarService.GetDefaultCalendarColorId(username),
                },
                Calendars = this.GetAllCalendarTitlesByUsername<CalendarEventViewModel>(username),
                Colors = this.colorService.GetAllColors(),
            };

            return model;
        }

        public async Task<bool> UpdateAsync(Event model, string eventId)
        {
            if (model == null || string.IsNullOrEmpty(eventId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var response = await this.searchService.UpdateIndexAsync<Event>(model);

            this.eventRepository.Update(model);
            var result = await this.eventRepository.SaveChangesAsync();

            return result > 0 && response == Result.Updated;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = await this.eventRepository.GetByIdWithDeletedAsync(id);

            if (eventFromDb == null)
            {
                throw new ArgumentException(string.Format(InvalidEventIdErrorMessage, id));
            }

            var response = await this.searchService.DeleteIndexAsync<Event>(eventFromDb);

            this.eventRepository.Delete(eventFromDb);
            var result = await this.eventRepository.SaveChangesAsync();

            return result > 0 && response == Result.Deleted;
        }

        public ICollection<Event> GetAll(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var all = this.eventRepository
                .All()
                .Where(x => x.Calendar.User.UserName == username)
                .ToList();
            return all;
        }

        public Event MapEventViewModelToEvent(EventViewModel model, string eventId)
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            if (string.IsNullOrEmpty(eventId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = this.eventRepository.All().Where(x => x.Id == eventId).First();

            eventFromDb.Id = eventId;
            eventFromDb.Title = model.Title;
            eventFromDb.Location = model.Location;
            eventFromDb.StartDateTime = model.StartDateTime;
            eventFromDb.EndDateTime = model.EndDateTime;
            eventFromDb.Description = model.Description;
            eventFromDb.CalendarId = model.CalendarId;
            eventFromDb.Coordinates = model.Coordinates != null ?
            model.Coordinates.Replace("(", string.Empty).Replace(")", string.Empty).Trim().ToString() :
            model.Coordinates;
            eventFromDb.ColorId = model.ColorId;

            return eventFromDb;
        }

        private ICollection<T> GetAllCalendarTitlesByUsername<T>(string username)
        {
            return this.calendarService.GetAllCalendarTitlesByUserName<T>(username);
        }
    }
}
