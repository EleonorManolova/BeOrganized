namespace OrganizeMe.Services.Data.Habits
{
    using OrganizeMe.Web.ViewModels.Habits;

    public interface IHabitService
    {
        T GetEnum<T>(string description);

        HabitCreateViewModel GetHabitViewModel();
    }
}
