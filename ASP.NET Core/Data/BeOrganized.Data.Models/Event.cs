namespace BeOrganized.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Common.Models;

    public class Event : BaseDeletableModel<string>
    {
        public Event()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string Location { get; set; }

        public string Coordinates { get; set; }

        [MaxLength(AttributesConstraints.EventDescriptionMaxLength)]
        public string Description { get; set; }

        public int ColorId { get; set; }

        public virtual Color Color { get; set; }

        [Required]
        public string CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }
    }
}
