namespace BeOrganized.Web.ViewModels.Golas
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;

    public class GoalCreateViewModel
    {
        public GoalInputViewModel Input { get; set; }

        public ICollection<string> DayTimes { get; set; }

        public ICollection<string> Frequencies { get; set; }

        public ICollection<string> Durations { get; set; }

        public ICollection<CalendarHabitViewModel> Calendars { get; set; } = new HashSet<CalendarHabitViewModel>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
