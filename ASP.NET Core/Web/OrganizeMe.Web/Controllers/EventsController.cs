namespace OrganizeMe.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Web.ViewModels.Events;

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
            var model = this.eventService.GetEventViewModel(this.configuration["GoogleMaps:ApiKey"]);
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
            return this.Redirect($"/Calendar");
        }
    }
}
