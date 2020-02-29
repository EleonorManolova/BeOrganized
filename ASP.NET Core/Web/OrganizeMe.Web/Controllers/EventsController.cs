namespace OrganizeMe.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using OrganizeMe.Web.ViewModels.Events;

    public class EventsController : Controller
    {
        private readonly IConfiguration configuration;

        public EventsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                Input = new Input
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                },
                GoogleApi = this.configuration["GoogleMaps:ApiKey"],
            };
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            return this.Redirect("/");
        }
    }
}
