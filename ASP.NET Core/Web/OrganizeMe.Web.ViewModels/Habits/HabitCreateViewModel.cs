namespace OrganizeMe.Web.ViewModels.Habits
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;

    public class HabitCreateViewModel
    {
        [BindProperty]
        public HabitInputViewModel Input { get; set; }

        public ICollection<string> DayTimes { get; set; }

        public ICollection<string> Frequencies { get; set; }

        public ICollection<string> Durations { get; set; }
    }
}
