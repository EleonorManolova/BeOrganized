namespace BeOrganized.Web.ViewModels.Administration.Goals
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class GoalModel : IMapFrom<Goal>
    {
        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [StringLength(AttributesConstraints.TitleMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringLengthMessage, MinimumLength = AttributesConstraints.TitleMinLength)]
        public string Title { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string Duration { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string Frequency { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string DayTime { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string CalendarId { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public int ColorId { get; set; }
    }
}
