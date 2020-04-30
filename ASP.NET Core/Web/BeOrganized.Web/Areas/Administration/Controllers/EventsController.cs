namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Web.ViewModels.Administration.Events;
    using BeOrganized.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : AdministrationController
    {
        private const string DeleteErrorMessage = "Failed to delete the event.";
        private const string DeleteSuccessMessage = "You successfully deleted event {0}!";

        private readonly IEventService eventService;
        private readonly UserManager<ApplicationUser> userManager;
        private ICalendarService calendarService;

        public EventsController(IEventService eventService, ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            this.eventService = eventService;
            this.calendarService = calendarService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return this.View(this.eventService.GetAll().OrderByDescending(x => x.CreatedOn).ToList());
        }

        public IActionResult Create()
        {
            var model = this.eventService.GetCreateViewModel();
            model.Users = this.userManager.Users.ToList();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(EventCreateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.CreateFromAdminAsync(model.EventModel);
            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet]
        public IActionResult Edit(string eventId, string calendarId)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(calendarId))
            {
                return this.BadRequest();
            }

            var username = this.calendarService.GetUserNameByCalendarId(calendarId);
            var eventFromDb = this.eventService.GetEditChangeViewModelById(eventId, username);
            if (eventFromDb == null)
            {
                return this.NotFound();
            }

            this.TempData["EventId"] = eventId;

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
            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet]
        [Route("/Administration/Events/Delete/{eventId}")]
        public async Task<IActionResult> Delete(string eventId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var eventModel = await this.eventService.GetByIdAsync(eventId);
            return this.View(eventModel);
        }

        [HttpPost]
        [Route("/Administration/Events/Delete/{eventId}")]
        public async Task<IActionResult> DeleteAsync(string eventId)
        {
            var eventTitle = (await this.eventService.GetByIdAsync(eventId)).Title;

            if (!await this.eventService.DeleteAsync(eventId))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
                this.View();
            }

            this.TempData["NotificationSuccess"] = string.Format(DeleteSuccessMessage, eventTitle);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
