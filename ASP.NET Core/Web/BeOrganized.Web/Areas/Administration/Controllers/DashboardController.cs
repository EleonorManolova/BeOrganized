namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Services.Data.Habit;
    using BeOrganized.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEventService eventService;
        private readonly IHabitService habitService;

        public DashboardController(UserManager<ApplicationUser> userManager, IEventService eventService, IHabitService habitService)
        {
            this.userManager = userManager;
            this.eventService = eventService;
            this.habitService = habitService;
        }

        public IActionResult Index()
        {
            var newUsers = this.userManager.Users.Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count();
            var newUsersForMonth = this.userManager.Users.Where(x => x.CreatedOn.Date > DateTime.Now.AddMonths(-1).Date).Count();
            var eventsCreatedToday = this.eventService.GetAll().Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count();
            var habitsCreatedToday = this.habitService.GetAll().Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count();
            var viewModel = new IndexViewModel
            {
                NewEventsToday = eventsCreatedToday,
                NewUsersThisMonth = newUsersForMonth,
                NewUsersToday = newUsers,
                NewHabitsToday = habitsCreatedToday,
            };
            return this.View(viewModel);
        }
    }
}
