namespace OrganizeMe.Web.Controllers
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Services.Data.Events;

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

        public async Task<IActionResult> IndexAsync()
        {
            var calendarId = await this.calendarService.GetDefaultCalendarIdAsync(this.User.Identity.Name);
            var events = await this.eventService.GetAllByCalendarIdAsync(calendarId);

            var eventsJson = JsonSerializer.Serialize(events);
            return this.View((object)eventsJson);
        }
    }
}
