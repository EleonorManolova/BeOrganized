namespace OrganizeMe.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;

    public class EventCreateViewModel
    {
        public EventInputViewModel Input { get; set; }

        public string GoogleApi { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; }
    }
}
