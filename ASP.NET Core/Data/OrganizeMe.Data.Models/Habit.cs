namespace OrganizeMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using OrganizeMe.Data.Common.Models;
    using OrganizeMe.Data.Models.Enums;

    public class Habit : BaseDeletableModel<string>
    {
        [Required]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        public Duration Duration { get; set; }

        [Required]
        public Frequency Frequency { get; set; }

        [Required]
        public DayTime DayTime { get; set; }

        public string CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }
    }
}
