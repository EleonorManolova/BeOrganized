namespace OrganizeMe.Web.ViewModels.Calendar
{
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class HabitCalendarViewModel : IMapFrom<Habit>
    {
        public string Title { get; set; }
    }
}
