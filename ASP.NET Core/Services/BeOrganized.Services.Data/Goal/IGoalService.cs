namespace BeOrganized.Services.Data.Goal
{
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Golas;

    public interface IGoalService
    {
        Task<bool> CreateAsync(GoalViewModel habitViewModel);

        T GetEnum<T>(string description);

        GoalChangeViewModel GetGoalViewModel(string username);

        GoalChangeViewModel GetGoalChangeViewModelById(string goalId, string username);

        Goal MapGoalViewModelToGoal(GoalViewModel model, string id);

        Task<bool> UpdateAsync(Goal goalModel, string habitId);

        Task<bool> DeleteAsync(string goalId);
    }
}
