namespace OrganizeMe.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class EventEditViewModel
    {
        public EventViewModel Output { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; } = new HashSet<CalendarEventViewModel>();
    }
}
