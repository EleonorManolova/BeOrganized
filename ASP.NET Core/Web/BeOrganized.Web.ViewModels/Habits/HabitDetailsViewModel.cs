namespace BeOrganized.Web.ViewModels.Habits
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HabitDetailsViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        [Display(Name = "Start DateTime")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End DateTime")]
        public DateTime EndDateTime { get; set; }

        [Display(Name = "Day Time")]
        public string GoalDayTime { get; set; }

        [Display(Name = "Frequency")]
        public string GoalFrequency { get; set; }

        [Display(Name = "Duration")]
        public string GoalDuration { get; set; }

        [Display(Name = "Calendar Title")]
        public string GoalCalendarTitle { get; set; }

        [Display(Name = "Color Hex")]
        public string GoalColorHex { get; set; }

        [Display(Name = "Goal Id")]
        public string GoalId { get; set; }
    }
}
