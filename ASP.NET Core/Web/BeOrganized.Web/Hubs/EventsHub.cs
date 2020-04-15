namespace BeOrganized.Web.Hubs
{
    using System;
    using System.Threading.Tasks;

    using BeOrganized.Services.Data.Events;
    using BeOrganized.Services.Data.Habit;
    using BeOrganized.Web.ViewModels.Events;
    using BeOrganized.Web.ViewModels.Habits;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class EventsHub : Hub
    {
        private readonly IEventService eventService;
        private readonly IHabitService habitService;

        public EventsHub(IEventService eventService, IHabitService habitService)
        {
            this.eventService = eventService;
            this.habitService = habitService;
        }

        public async Task EventsChange(EventHubViewModel model)
        {
            var eventViewModel = await this.eventService.GetByIdAsync(model.Id);
            eventViewModel.StartDateTime = DateTime.Parse(model.StartDateTime);
            eventViewModel.EndDateTime = DateTime.Parse(model.EndDateTime);
            await this.eventService.UpdateAsync(eventViewModel, model.Id);
        }

        public async Task HabitsChange(HabitHubViewModel model)
        {
            var habitsViewModel = await this.habitService.GetByIdAsync(model.Id);
            habitsViewModel.StartDateTime = DateTime.Parse(model.StartDateTime);
            habitsViewModel.EndDateTime = DateTime.Parse(model.EndDateTime);
            await this.habitService.UpdateAsync(habitsViewModel, model.Id);
        }
    }
}
