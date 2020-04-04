namespace OrganizeMe.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class EventCreateViewModel
    {
        public EventViewModel Input { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; } = new HashSet<CalendarEventViewModel>();
    }
}
