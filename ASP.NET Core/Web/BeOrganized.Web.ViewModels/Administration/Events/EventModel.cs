namespace BeOrganized.Web.ViewModels.Administration.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class EventModel : IValidatableObject, IMapFrom<Event>
    {
        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [StringLength(AttributesConstraints.TitleMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringLengthMessage, MinimumLength = AttributesConstraints.TitleMinLength)]
        public string Title { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Start Date Time")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "End Date Time")]
        public DateTime EndDateTime { get; set; }

        public string Location { get; set; }

        public string Coordinates { get; set; }

        [MaxLength(AttributesConstraints.EventDescriptionMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringMaxLengthMessage)]
        public string Description { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Calendar")]
        public string CalendarId { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public int ColorId { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (this.StartDateTime > this.EndDateTime)
            {
                yield return new ValidationResult("The start day and time must be before the end day and time.");
            }
        }
    }
}