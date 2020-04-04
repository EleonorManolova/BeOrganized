namespace OrganizeMe.Services.Data.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Nest;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Services.Data.Color;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;

    public class EventService : IEventService
    {
        private const string InvalidEventIdErrorMessage = "Event with Id: {0} does not exist.";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";
        private const string InvalidDateTimeErrorMessage = "The start day and time must be before the end day and time.";

        private readonly IDeletableEntityRepository<Event> eventRepository;
        private readonly ICalendarService calendarService;
        private readonly IElasticClient elasticClient;
        private readonly IColorService colorService;

        public EventService(IDeletableEntityRepository<Event> eventRepository, ICalendarService calendarService, IElasticClient elasticClient, IColorService colorService)
        {
            this.eventRepository = eventRepository;
            this.calendarService = calendarService;
            this.elasticClient = elasticClient;
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

            await this.eventRepository.AddAsync(eventFromForm);
            var result = await this.eventRepository.SaveChangesAsync();

            // var indexDescriptor = new CreateIndexDescriptor("events").Mappings(ms => ms.Map<Event>(m => m.AutoMap()));
            // var resp = this.elasticClient.Index(eventFromForm, m => m.Index("events"));
            return result > 0;
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

        public async Task<ICollection<EventCalendarViewModel>> GetAllByCalendarIdAsync(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var events = await this.eventRepository.All().Where(x => x.CalendarId == calendarId).To<EventCalendarViewModel>().ToListAsync();
            return events;
        }

        public async Task<EventEditViewModel> GetEditViewModelByIdAsync(string eventId, string username)
        {
            if (string.IsNullOrEmpty(eventId) ||
               string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var eventFromDb = await this.eventRepository.All().Where(x => x.Id == eventId).To<EventViewModel>().FirstAsync();
            var eventResult = new EventEditViewModel
            {
                Output = eventFromDb,
                Calendars = await this.GetAllCalendarTitlesByUsernameAsync<CalendarEventViewModel>(username),
            };
            return eventResult;
        }

        public async Task<EventCreateViewModel> GetCreateViewModelAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var model = new EventCreateViewModel
            {
                Input = new EventViewModel
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    ColorId = await this.calendarService.GetDefaultCalendarColorIdAsync(username),
                },
                Calendars = await this.GetAllCalendarTitlesByUsernameAsync<CalendarEventViewModel>(username),
                Colors = await this.colorService.GetAllColorsAsync(),
            };

            return model;
        }

        public async Task<bool> UpdateAsync(EventEditViewModel model, string eventId)
        {
            if (model == null || string.IsNullOrEmpty(eventId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

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
                ColorId = model.Output.ColorId,
            };

            this.eventRepository.Update(eventNew);
            var result = await this.eventRepository.SaveChangesAsync();

            // await this.elasticClient.UpdateAsync<Event>(eventNew, u => u.Doc(eventNew));
            return result > 0;
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
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            this.eventRepository.Delete(eventFromDb);
            var result = await this.eventRepository.SaveChangesAsync();

            // await this.elasticClient.DeleteAsync<Event>(eventFromDb);
            return result > 0;
        }

        public async Task<ICollection<Event>> GetAllAsync(string username)
        {
            var all = await this.eventRepository.All().Where(x => x.Calendar.User.UserName == username).ToListAsync();
            return all;
        }

        private async Task<ICollection<T>> GetAllCalendarTitlesByUsernameAsync<T>(string username)
        {
            return await this.calendarService.GetAllCalendarTitlesByUserIdAsync<T>(username);
        }
    }
}
