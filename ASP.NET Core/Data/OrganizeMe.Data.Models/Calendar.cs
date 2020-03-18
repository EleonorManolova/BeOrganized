namespace OrganizeMe.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using OrganizeMe.Common;
    using OrganizeMe.Data.Common.Models;

    public class Calendar : BaseDeletableModel<int>
    {
        public Calendar()
        {
            this.Events = new HashSet<Event>();
            this.Habits = new HashSet<Habit>();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<Habit> Habits { get; set; }
    }
}
