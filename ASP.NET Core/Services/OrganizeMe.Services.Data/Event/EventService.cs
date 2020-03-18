namespace OrganizeMe.Services.Data.Events
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Web.ViewModels.Events;

    public class EventService : IEventService
    {
        private readonly IDeletableEntityRepository<Event> eventRepository;

        public EventService(IDeletableEntityRepository<Event> eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<bool> CreateAsync(EventInputViewModel eventViewModel)
        {
            // TODO:check if it works
            Event eventFromForm = new Event
            {
                Title = eventViewModel.Title,
                Location = eventViewModel.Location,
                StartDate = eventViewModel.StartDate,
                StartTime = eventViewModel.StartTime,
                EndDate = eventViewModel.EndDate,
                EndTime = eventViewModel.EndTime,
                Description = eventViewModel.Description,
            };

            await this.eventRepository.AddAsync(eventFromForm);
            await this.eventRepository.SaveChangesAsync();
            return true;
        }

        public EventCreateViewModel GetEventViewModel(string googleApi)
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
            };

            return model;
        }
    }
}
