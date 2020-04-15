namespace BeOrganized.Web.Controllers
{
    using System.Threading.Tasks;

    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Web.ViewModels.Golas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class GoalsController : BaseController
    {
        private readonly IGoalService goalService;
        private readonly IEnumParseService enumParseService;

        public GoalsController(IGoalService goalService, IEnumParseService enumParseService)
        {
            this.goalService = goalService;
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
        public async Task<IActionResult> CreateAsync(GoalChangeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (!this.enumParseService.IsEnumValid<DayTime>(model.GoalModel.DayTime.ToString()) ||
                !this.enumParseService.IsEnumValid<Frequency>(model.GoalModel.Frequency.ToString()) ||
                !this.enumParseService.IsEnumValid<Duration>(model.GoalModel.Duration.ToString()))
            {
                return this.View(model);
            }

            await this.goalService.CreateAsync(model.GoalModel);
            return this.Redirect("/");
        }

        [HttpGet]
        public IActionResult Edit(string habitId, string goalId)
        {
            if (string.IsNullOrEmpty(habitId) || string.IsNullOrEmpty(goalId))
            {
                return this.BadRequest();
            }

            var goal = this.goalService.GetGoalChangeViewModelById(goalId, this.User.Identity.Name);
            if (goal == null)
            {
                return this.NotFound();
            }

            this.TempData["GoalId"] = goalId;
            this.TempData["HabitId"] = habitId;

            return this.View(goal);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(GoalChangeViewModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var habitId = this.TempData["HabitId"].ToString();
            var goalModel = this.goalService.MapGoalViewModelToGoal(model.GoalModel, id);
            await this.goalService.UpdateAsync(goalModel, habitId);
            return this.Redirect("/Calendar");
        }
    }
}
