namespace OrganizeMe.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using OrganizeMe.Common;
    using OrganizeMe.Data.Common.Models;

    public class Event : BaseDeletableModel<string>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        [MaxLength(AttributesConstraints.DescriptionMaxLength)]
        public string Description { get; set; }

        public string CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }
    }
}
