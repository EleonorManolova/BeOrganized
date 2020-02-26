namespace OrganizeMe.Web.Controllers
{
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
                GoogleApi = this.configuration["GoogleMaps:ApiKey"],
            };
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel model)
        {
            return this.Redirect("/");
        }
    }
}
