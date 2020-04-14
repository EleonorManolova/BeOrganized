namespace BeOrganized.Web.ViewModels.Calendar
{
    using System.Collections.Generic;

    public class CalendarIndexViewModel
    {
        public CalendarIndexViewModel()
        {
            this.Events = new HashSet<EventCalendarViewModel>();
            this.Habits = new HashSet<HabitCalendarViewModel>();
        }

        public ICollection<EventCalendarViewModel> Events { get; set; }

         public ICollection<HabitCalendarViewModel> Habits { get; set; }
    }
}
