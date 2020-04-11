namespace BeOrganized.Web.Hubs
{
    using System;
    using System.Threading.Tasks;

    using BeOrganized.Services.Data.Events;
    using BeOrganized.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class EventsHub : Hub
    {
        private readonly IEventService eventService;

        public EventsHub(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public async Task EventsChange(EventHubViewModel model)
        {
            var eventViewModel = await this.eventService.GetByIdAsync(model.Id);
            eventViewModel.StartDateTime = DateTime.Parse(model.StartDateTime);
            eventViewModel.EndDateTime = DateTime.Parse(model.EndDateTime);
            await this.eventService.UpdateAsync(eventViewModel, model.Id);
        }
    }
}
