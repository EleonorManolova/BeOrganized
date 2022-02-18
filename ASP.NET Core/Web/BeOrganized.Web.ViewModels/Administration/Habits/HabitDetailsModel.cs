namespace BeOrganized.Web.ViewModels.Administration.Habits
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HabitDetailsModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        [Display(Name = "Start DateTime")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End DateTime")]
        public DateTime EndDateTime { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
