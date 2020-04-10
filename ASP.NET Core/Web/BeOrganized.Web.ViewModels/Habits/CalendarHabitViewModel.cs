namespace BeOrganized.Web.ViewModels.Habits
{
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class CalendarHabitViewModel : IMapFrom<Calendar>
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}
