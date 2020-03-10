namespace OrganizeMe.Web.ViewModels.Habits
{
    using System.ComponentModel.DataAnnotations;

    using OrganizeMe.Common;
    using OrganizeMe.Data.Models.Enums;

    public class HabitInputViewModel
    {
        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string Duration { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public Frequency Frequency { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public DayTime DayTime { get; set; }
    }
}
