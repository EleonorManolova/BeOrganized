namespace BeOrganized.Web.Controllers
{
    using System.Threading.Tasks;

    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Web.ViewModels.Golas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class GoalsController : BaseController
    {
        private readonly IGoalService goalService;
        private readonly IEnumParseService enumParseService;

        public GoalsController(IGoalService habitService, IEnumParseService enumParseService)
        {
            this.goalService = habitService;
            this.enumParseService = enumParseService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            var model = this.goalService.GetGoalViewModel(this.User.Identity.Name);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GoalCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (!this.enumParseService.IsEnumValid<DayTime>(model.Input.DayTime) || !this.enumParseService.IsEnumValid<Frequency>(model.Input.Frequency) || !this.enumParseService.IsEnumValid<Duration>(model.Input.Duration))
            {
                return this.View(model);
            }

            await this.goalService.CreateAsync(model.Input);
            return this.Redirect("/");
        }

        [HttpGet]
        public IActionResult Edit(string habitId, string goalId)
        {
            if (string.IsNullOrEmpty(habitId) || string.IsNullOrEmpty(goalId))
            {
                return this.BadRequest();
            }

            var goal = this.goalService.GetGoalCreateViewModelById(habitId, goalId);
            if (goal == null)
            {
                return this.NotFound();
            }

            this.TempData["HabitId"] = habitId;

            return this.View(goal);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(GoalCreateViewModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            //var goalModel = this.goalService.MapEventViewModelToEvent(model, id);
            //await this.goalService.UpdateAsync(goalModel, id);
            return this.Redirect("/Calendar");
        }
    }
}
