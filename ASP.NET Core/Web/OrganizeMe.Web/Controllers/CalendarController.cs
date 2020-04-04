namespace OrganizeMe.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Data.Calendar;

    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ICalendarService calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            this.calendarService = calendarService;
        }

        public IActionResult Index()
        {
            var model = this.calendarService.GetDefaultCalendarIndexViewModel(this.User.Identity.Name);
            return this.View(model);
        }
    }
}
