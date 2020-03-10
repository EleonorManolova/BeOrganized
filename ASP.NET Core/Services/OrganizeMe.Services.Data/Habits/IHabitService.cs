namespace OrganizeMe.Services.Data.Habits
{
    using OrganizeMe.Web.ViewModels.Habits;

    public interface IHabitService
    {
       HabitCreateViewModel GetHabitViewModel();
    }
}
