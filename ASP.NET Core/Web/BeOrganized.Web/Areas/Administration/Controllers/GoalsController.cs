using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeOrganized.Data.Models;
using BeOrganized.Services.Data.Goal;
using BeOrganized.Services.Data.Habit;
using BeOrganized.Web.ViewModels.Administration.Goals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeOrganized.Web.Areas.Administration.Controllers
{
    public class GoalsController : AdministrationController
    {
        private const string DeleteErrorMessage = "Failed to delete the goal.";
        private const string DeleteSuccessMessage = "You successfully deleted goal {0}!";

        private readonly IGoalService goalService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHabitService habitService;

        public GoalsController(IGoalService goalService, UserManager<ApplicationUser> userManager, IHabitService habitService)
        {
            this.goalService = goalService;
            this.userManager = userManager;
            this.habitService = habitService;
        }

        public IActionResult Index()
        {
            var goals = this.goalService.GetDetailsViewModels().OrderByDescending(x => x.CreatedOn).ToList();
            return this.View(goals);
        }

        public IActionResult Create()
        {
            var model = this.goalService.GetCreateViewModel();
            model.Users = this.userManager.Users.ToList();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GoalChangeModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.goalService.CreateFromAdminAsync(model.GoalModel);
            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Edit(string goalId)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                return this.BadRequest();
            }

            var calendar = this.goalService.GetEditChangeViewModelById(goalId);
            if (calendar == null)
            {
                return this.NotFound();
            }

            calendar.Users = this.userManager.Users.ToList();
            //this.TempData["HabitId"] = habitId;
            this.TempData["GoalId"] = goalId;

            return this.View(calendar);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(GoalChangeModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var habitId = this.TempData["HabitId"].ToString();
            var goalModel = this.goalService.MapGoalModelToGoal(model.GoalModel, id);
            await this.goalService.UpdateFromAdminAsync(goalModel);
            return this.RedirectToAction(nameof(this.Index));
        }

        [Route("/Administration/Goals/Delete/{goalId}")]
        public async Task<IActionResult> Delete(string goalId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var goal = await this.goalService.GetByIdAsync(goalId);
            return this.View(goal);
        }

        [HttpPost]
        [Route("/Administration/Goals/Delete/{goalId}")]
        public async Task<IActionResult> DeleteAsync(string goalId)
        {
            var goal = (await this.goalService.GetByIdAsync(goalId));

            if (!await this.goalService.DeleteAsync(goalId))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
                this.View();
            }

            var habit = this.habitService.GetAllByCalendarId(goal.CalendarId)
                .OrderBy(x => x.StartDateTime)
                .First();

            if (!await this.habitService.DeleteFollowingAsync(habit.Id))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
            }

            this.TempData["NotificationSuccess"] = string.Format(DeleteSuccessMessage, goal.Title);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}