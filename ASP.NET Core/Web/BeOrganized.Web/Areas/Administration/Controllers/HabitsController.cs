namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System.Linq;

    using BeOrganized.Services.Data.Habit;
    using Microsoft.AspNetCore.Mvc;

    public class HabitsController : AdministrationController
    {
        private readonly IHabitService habitService;

        public HabitsController(IHabitService habitService)
        {
            this.habitService = habitService;
        }

        public IActionResult Index()
        {
            var calendars = this.habitService.GetDetailsViewModels().OrderByDescending(x => x.CreatedOn).ToList();
            return this.View(calendars);
        }
    }
}