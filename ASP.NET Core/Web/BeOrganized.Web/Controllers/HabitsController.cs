namespace BeOrganized.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Services.Data.Habit;
    using Microsoft.AspNetCore.Mvc;

    public class HabitsController : BaseController
    {
        private readonly IHabitService habitService;

        public HabitsController(IHabitService habitService)
        {
            this.habitService = habitService;
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/");
            }

            var model = this.habitService.GetDetailsViewModelById(id);
            return this.PartialView("_HabitDetailsPartial", model);
        }
    }
}
