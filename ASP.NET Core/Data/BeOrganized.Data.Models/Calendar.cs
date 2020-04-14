namespace BeOrganized.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Common.Models;

    public class Calendar : BaseDeletableModel<string>
    {
        public Calendar()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Events = new HashSet<Event>();
            this.Goals = new HashSet<Goal>();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public int DefaultCalendarColorId { get; set; }

        public virtual Color DefaultCalendarColor { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }
    }
}
