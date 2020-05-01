namespace BeOrganized.Web.ViewModels.Administration.Goals
{
    using System;

    using BeOrganized.Data.Models.Enums;

    public class GoalDetailsModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public Duration Duration { get; set; }

        public Frequency Frequency { get; set; }

        public DayTime DayTime { get; set; }

        public DateTime StartDateTime { get; set; }

        public bool IsActive { get; set; }

        public string ColorName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
