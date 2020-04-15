namespace BeOrganized.Web.Controllers
{
    using System.Threading.Tasks;

    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Services.Data.Habit;
    using Microsoft.AspNetCore.Mvc;

    public class HabitsController : BaseController
    {
        private const string CompleteSuccessMessage = "The habit {0} is set to completed.";
        private const string NotCompleteSuccessMessage = "The habit {0} is set to not completed.";
        private const string UpdateErrorMessage = "Failed to update habit";
        private const string DeleteErrorMessage = "Failed to delete the habit.";
        private const string DeleteSuccessMessage = "You successfully deleted habit {0}!";

        private readonly IHabitService habitService;
        private readonly IGoalService goalService;

        public HabitsController(IHabitService habitService, IGoalService goalService)
        {
            this.habitService = habitService;
            this.goalService = goalService;
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

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/");
            }

            var model = await this.habitService.GetByIdAsync(id);
            return this.PartialView("_HabitDeletePartial", model);
        }

        [HttpPost]
        [Route("/Habits/DeleteCurrent/{id}")]
        public async Task<IActionResult> DeleteCurrent(string id)
        {
            var habitTitle = (await this.habitService.GetByIdAsync(id)).Title;

            if (!await this.habitService.SetComplete(id))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
            }

            this.TempData["NotificationSuccess"] = string.Format(CompleteSuccessMessage, habitTitle);

            return this.Redirect("/Calendar");
        }

        [HttpPost]
        [Route("/Habits/DeleteFollowing/{id}")]
        public async Task<IActionResult> DeleteFollowing(string id)
        {
            var habit = await this.habitService.GetByIdAsync(id);

            if (!await this.habitService.DeleteFollowingAsync(id))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
            }

            await this.goalService.DeleteAsync(habit.GoalId);
            this.TempData["NotificationSuccess"] = string.Format(DeleteSuccessMessage, habit.Title);

            return this.Redirect("/Calendar");
        }

        [HttpPost]
        [Route("/Habits/Complete/{id}")]
        public async Task<IActionResult> Complete(string id)
        {
            var habitTitle = (await this.habitService.GetByIdAsync(id)).Title;

            if (!await this.habitService.SetComplete(id))
            {
                this.TempData["NotificationError"] = UpdateErrorMessage;
            }

            this.TempData["NotificationSuccess"] = string.Format(CompleteSuccessMessage, habitTitle);

            return this.Redirect("/Calendar");
        }

        [HttpPost]
        public async Task<IActionResult> NotComplete(string id)
        {
            var habitTitle = (await this.habitService.GetByIdAsync(id)).Title;

            if (!await this.habitService.SetNotComplete(id))
            {
                this.TempData["NotificationError"] = UpdateErrorMessage;
            }

            this.TempData["NotificationSuccess"] = string.Format(NotCompleteSuccessMessage, habitTitle);

            return this.Redirect("/Calendar");
        }
    }
}
