namespace OrganizeMe.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using OrganizeMe.Common;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class EventViewModel : IValidatableObject, IMapFrom<Event>, IHaveCustomMappings
    {
        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [StringLength(AttributesConstraints.TitleMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringLengthMessage, MinimumLength = AttributesConstraints.TitleMinLength)]
        public string Title { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = AttributesConstraints.EventDateFromat)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = AttributesConstraints.EventDateFromat)]
        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        public string Coordinates { get; set; }

        [MaxLength(AttributesConstraints.EventDescriptionMaxLength, ErrorMessage = AttributesErrorMessages.PasswordStringMaxLengthMessage)]
        public string Description { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        [Display(Name = "Calendar")]
        public string CalendarId { get; set; }

        [Required(ErrorMessage = AttributesErrorMessages.RequiredErrorMessage)]
        public int ColorId { get; set; }

        public DateTime StartDateTime => new DateTime(this.StartDate.Year, this.StartDate.Month, this.StartDate.Day, this.StartTime.Hour, this.StartTime.Minute, this.StartTime.Second);

        public DateTime EndDateTime => new DateTime(this.EndDate.Year, this.EndDate.Month, this.EndDate.Day, this.EndTime.Hour, this.EndTime.Minute, this.EndTime.Second);

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventViewModel>()
                .ForMember(x => x.StartDate, y => y.MapFrom(x => x.StartDateTime.Date))
                .ForMember(x => x.StartTime, y => y.MapFrom(x => default(DateTime).Add(x.StartDateTime.TimeOfDay)))
                .ForMember(x => x.EndDate, y => y.MapFrom(x => x.EndDateTime.Date))
                .ForMember(x => x.EndTime, y => y.MapFrom(x => default(DateTime).Add(x.EndDateTime.TimeOfDay)));
        }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (this.StartDateTime > this.EndDateTime)
            {
                yield return new ValidationResult("The start day and time must be before the end day and time.");
            }
        }
    }
}
