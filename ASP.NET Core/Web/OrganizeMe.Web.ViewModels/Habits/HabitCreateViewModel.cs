namespace OrganizeMe.Web.ViewModels.Habits
{
    using System.Collections.Generic;

    public class HabitCreateViewModel
    {
        public HabitInputViewModel Input { get; set; }

        public ICollection<string> DayTimes { get; set; }

        public ICollection<string> Frequencies { get; set; }

        public ICollection<string> Durations { get; set; }

        public ICollection<CalendarHabitViewModel> Calendars { get; set; }
    }
}
