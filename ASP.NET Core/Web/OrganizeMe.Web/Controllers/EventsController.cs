namespace OrganizeMe.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Web.ViewModels.Events;

    [Authorize]
    public class EventsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IEventService eventService;

        public EventsController(IConfiguration configuration, IEventService eventService)
        {
            this.configuration = configuration;
            this.eventService = eventService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = this.eventService.GetEventViewModel(this.User.Identity.Name);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.CreateAsync(model.Input);
            return this.Redirect("/Calendar");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            var eventFromDb = this.eventService.GetEventById(id, this.User.Identity.Name);
            if (eventFromDb == null)
            {
                return this.NotFound();
            }

            this.TempData["EventId"] = id;

            return this.View(eventFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel model)
        {
            var id = this.TempData["EventId"].ToString();
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.UpdateEvent(model, id);
            return this.Redirect("/Calendar");
        }
    }
}
