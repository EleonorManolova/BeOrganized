namespace BeOrganized.Web.Controllers
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Events;

    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ICalendarService calendarService;
        private readonly IEventService eventService;

        public CalendarController(ICalendarService calendarService, IEventService eventService)
        {
            this.calendarService = calendarService;
            this.eventService = eventService;
        }

        public IActionResult Index()
        {
            var calendarId = this.calendarService.GetDefaultCalendarId(this.User.Identity.Name);
            var events = this.eventService.GetAllByCalendarId(calendarId);

            var eventsJson = JsonSerializer.Serialize(events);
            return this.View((object)eventsJson);
        }
    }
}
