namespace BeOrganized.Web.ViewModels.Events
{
    using System;

    public class HabitDetailsViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public string CalendarTitle { get; set; }

        public string ColorHex { get; set; }
    }
}
