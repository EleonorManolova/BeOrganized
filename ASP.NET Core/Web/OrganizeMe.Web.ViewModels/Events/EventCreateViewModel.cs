namespace OrganizeMe.Web.ViewModels.Events
{
    using OrganizeMe.Data.Models;
    using System.Collections.Generic;

    public class EventCreateViewModel
    {
        public EventViewModel Input { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; } = new HashSet<CalendarEventViewModel>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
