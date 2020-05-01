namespace BeOrganized.Web.ViewModels.Administration.Goals
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;

    public class GoalChangeModel
    {
        public GoalModel GoalModel { get; set; }

        public ICollection<string> DayTimes { get; set; }

        public ICollection<string> Frequencies { get; set; }

        public ICollection<string> Durations { get; set; }

        public ICollection<Calendar> Calendars { get; set; } = new HashSet<Calendar>();

        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
