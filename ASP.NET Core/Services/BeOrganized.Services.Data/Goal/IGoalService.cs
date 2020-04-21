namespace BeOrganized.Services.Data.Goal
{
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Golas;

    public interface IGoalService
    {
        Task<bool> CreateAsync(GoalViewModel habitViewModel);

        GoalChangeViewModel GetGoalChangeViewModel(string username);

        GoalChangeViewModel GetGoalChangeViewModelById(string goalId, string username);

        Goal MapGoalViewModelToGoal(GoalViewModel model, string id);

        Task<bool> UpdateAsync(Goal goalModel, string habitId);

        Task<bool> DeleteAsync(string goalId);

        Task CreateMoreHabitsAsync(string calendarId);
    }
}
