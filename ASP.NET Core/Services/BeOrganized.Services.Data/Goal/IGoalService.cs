namespace BeOrganized.Services.Data.Goal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Administration.Goals;
    using BeOrganized.Web.ViewModels.Golas;

    public interface IGoalService
    {
        Task<bool> CreateAsync(GoalViewModel habitViewModel);

        GoalChangeViewModel GetGoalChangeViewModel(string username);

        GoalChangeViewModel GetGoalChangeViewModelById(string goalId, string username);

        Goal MapGoalViewModelToGoal(GoalViewModel model, string id);

        Task<bool> UpdateAsync(Goal goalModel, string habitId);

        ICollection<GoalDetailsModel> GetDetailsViewModels();

        Task<bool> DeleteAsync(string goalId);

        Task CreateMoreHabitsAsync(string calendarId);

        GoalChangeModel GetCreateViewModel();

        Task<bool> CreateFromAdminAsync(GoalModel goalModel);

        GoalChangeModel GetEditChangeViewModelById(string goalId);

        Goal MapGoalModelToGoal(GoalModel model, string goalId);

        Task<Goal> GetByIdAsync(string id);
        Task<bool> UpdateFromAdminAsync(Goal model);
    }
}
