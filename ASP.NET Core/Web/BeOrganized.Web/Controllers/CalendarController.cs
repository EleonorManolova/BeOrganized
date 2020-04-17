namespace BeOrganized.Web.Controllers
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Services.Data.Habit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class CalendarController : BaseController
    {
        private readonly ICalendarService calendarService;
        private readonly IEventService eventService;
        private readonly IHabitService habitService;
        private readonly UserManager<ApplicationUser> signInManager;
        private readonly IGoalService goalService;

        public CalendarController(ICalendarService calendarService, IEventService eventService, IHabitService habitService, UserManager<ApplicationUser> signInManager, IGoalService goalService)
        {
            this.calendarService = calendarService;
            this.eventService = eventService;
            this.habitService = habitService;
            this.signInManager = signInManager;
            this.goalService = goalService;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(this.signInManager.GetUserId(this.User)))
            {
                this.Redirect("/");
            }

            var calendarId = this.calendarService.GetDefaultCalendarId(this.User.Identity.Name);

            // Generate Habit for month ahead
            await this.goalService.CreateMoreHabitsAsync(calendarId);

            var events = this.eventService.GetAllByCalendarId(calendarId);
            var habits = this.habitService.GetAllByCalendarId(calendarId);

            var eventsJson = JsonSerializer.Serialize(events);
            var habitsJson = JsonSerializer.Serialize(habits);
            this.ViewBag.EventsJson = eventsJson;
            this.ViewBag.HabitsJson = habitsJson;
            return this.View();
        }
    }
}
