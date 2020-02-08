namespace OrganizeMe.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult CreateConfirm()
        {
            return this.Redirect("/");
        }
    }
}
