namespace OrganizeMe.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using OrganizeMe.Common;
    using OrganizeMe.Data.Common.Models;
    using OrganizeMe.Data.Models.Enums;

    public class Habit : BaseDeletableModel<string>
    {
        public Habit()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public Duration Duration { get; set; }

        [Required]
        public Frequency Frequency { get; set; }

        [Required]
        public DayTime DayTime { get; set; }

        public int ColorId { get; set; }

        public virtual Color Color { get; set; }

        [Required]
        public string CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }
    }
}
