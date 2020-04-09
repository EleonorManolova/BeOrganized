namespace OrganizeMe.Web.Hubs
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Web.ViewModels.Events;

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

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
