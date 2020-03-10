namespace OrganizeMe.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Services.Data.Habits;
    using OrganizeMe.Web.ViewModels.Habits;

    public class HabitsController : Controller
    {
        private readonly IHabitService habitService;

        public HabitsController(IHabitService habitService)
        {
            this.habitService = habitService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            var model = this.habitService.GetHabitViewModel();
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Create(HabitCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // saveToDb
            return this.Redirect("/");
        }
    }
}
