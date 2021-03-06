﻿namespace BeOrganized.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BeOrganized.Common;
    using BeOrganized.Data.Common.Models;
    using BeOrganized.Data.Models.Enums;

    public class Habit : BaseDeletableModel<string>
    {
        public Habit()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MinLength(AttributesConstraints.TitleMinLength)]
        [MaxLength(AttributesConstraints.TitleMaxLength)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string GoalId { get; set; }

        public virtual Goal Goal { get; set; }
    }
}
