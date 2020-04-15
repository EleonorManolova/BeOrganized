namespace BeOrganized.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Common.Models;
    using BeOrganized.Data.Models.Enums;

    public class Goal : BaseDeletableModel<string>
    {
        public Goal()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public Duration Duration { get; set; }

        [Required]
        public Frequency Frequency { get; set; }

        [Required]
        public DayTime DayTime { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        public bool IsActive { get; set; }

        public int ColorId { get; set; }

        public virtual Color Color { get; set; }

        [Required]
        public string CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }

        public ICollection<Habit> Habits { get; set; }
    }
}
