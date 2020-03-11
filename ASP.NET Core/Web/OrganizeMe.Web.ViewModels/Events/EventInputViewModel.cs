namespace OrganizeMe.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Common;

    public class EventInputViewModel : IValidatableObject
    {
        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public string Title { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = AttributesConstraints.DateFromat)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = AttributesConstraints.DateFromat)]
        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        [MaxLength(AttributesConstraints.DescriptionMaxLength)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var startDateTime = new DateTime(this.StartDate.Year, this.StartDate.Month, this.StartDate.Day, this.StartTime.Hour, this.StartTime.Minute, this.StartTime.Second);
            var endDateTime = new DateTime(this.EndDate.Year, this.EndDate.Month, this.EndDate.Day, this.EndTime.Hour, this.EndTime.Minute, this.EndTime.Second);

            if (startDateTime > endDateTime)
            {
                yield return new ValidationResult("The start day and time must be before the end day and time.");
            }
        }
    }
}
