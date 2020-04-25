namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEventService eventService;

        public DashboardController(UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            this.userManager = userManager;
            this.eventService = eventService;
        }

        public IActionResult Index()
        {
            var newUsers = this.userManager.Users.Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count();
            var newUsersForMonth = this.userManager.Users.Where(x => x.CreatedOn.Date > DateTime.Now.AddMonths(-1).Date).Count();
            var eventsCreatedToday = this.eventService.GetAll().Where(x => x.CreatedOn.Date == DateTime.Now.Date).Count();
            var viewModel = new IndexViewModel
            {
                NewEventsToday = eventsCreatedToday,
                NewUsersThisMonth = newUsersForMonth,
                NewUsersToday = newUsers,
            };
            return this.View(viewModel);
        }
    }
}
