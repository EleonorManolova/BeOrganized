namespace OrganizeMe.Web.ViewModels.Habits
{
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class CalendarHabitViewModel : IMapFrom<Calendar>
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}
