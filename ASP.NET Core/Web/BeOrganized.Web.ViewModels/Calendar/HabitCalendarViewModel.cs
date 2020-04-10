namespace BeOrganized.Web.ViewModels.Calendar
{
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class HabitCalendarViewModel : IMapFrom<Habit>
    {
        public string Title { get; set; }
    }
}
