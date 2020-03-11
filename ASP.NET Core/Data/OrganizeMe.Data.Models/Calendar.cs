namespace OrganizeMe.Data.Models
{
    using System.Collections.Generic;

    using OrganizeMe.Data.Common.Models;

    public class Calendar : BaseDeletableModel<string>
    {
        public Calendar()
        {
            this.Events = new HashSet<Event>();
            this.Habits = new HashSet<Habit>();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<Habit> Habits { get; set; }
    }
}
