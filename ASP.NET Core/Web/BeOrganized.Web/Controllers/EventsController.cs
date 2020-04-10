namespace BeOrganized.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Web.Hubs;
    using BeOrganized.Web.ViewModels.Events;

    [Authorize]
    public class EventsController : Controller
    {
        private const string DeleteErrorMessage = "Failed to delete the event.";
        private const string DeleteSuccessMessage = "You successfully deleted event {0} !";

        private readonly IEventService eventService;
        private readonly IHubContext<EventsHub> eventsHub;

        public EventsController(IEventService eventService, IHubContext<EventsHub> eventsHub)
        {
            this.eventService = eventService;
            this.eventsHub = eventsHub;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = this.eventService.GetCreateChangeViewModel(this.User.Identity.Name);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(EventChangeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.CreateAsync(model.EventModel);
            return this.Redirect("/Calendar");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            var eventFromDb = this.eventService.GetEditChangeViewModelById(id, this.User.Identity.Name);
            if (eventFromDb == null)
            {
                return this.NotFound();
            }

            this.TempData["EventId"] = id;

            return this.View(eventFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EventChangeViewModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var eventModel = this.eventService.MapEventViewModelToEvent(model.EventModel, id);
            await this.eventService.UpdateAsync(eventModel, id);
            return this.Redirect("/Calendar");
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/");
            }

            var model = this.eventService.GetDetailsViewModelById(id);
            return this.PartialView("_EventDetailsPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/");
            }

            var model = await this.eventService.GetByIdAsync(id);
            return this.PartialView("_EventDeletePartial", model);
        }

        [HttpPost]
        [Route("/Events/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmAsync(string id)
        {
            var eventTitle = (await this.eventService.GetByIdAsync(id)).Title;

            if (!await this.eventService.DeleteAsync(id))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;

                return this.Redirect($"/Events/Delete/{id}");
            }

            this.TempData["NotificationSuccess"] = string.Format(DeleteSuccessMessage, eventTitle);

            return this.Redirect("/Calendar");
        }
    }
}
