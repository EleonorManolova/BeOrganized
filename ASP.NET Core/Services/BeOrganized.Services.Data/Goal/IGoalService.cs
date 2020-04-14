namespace BeOrganized.Services.Data.Goal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Golas;

    public interface IGoalService
    {
        Task<bool> CreateAsync(GoalInputViewModel habitViewModel);

        T GetEnum<T>(string description);

        GoalCreateViewModel GetGoalViewModel(string username);
    }
}
