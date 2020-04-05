namespace OrganizeMe.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using OrganizeMe.Data.Models;

    public class EventCreateViewModel
    {
        public EventViewModel Input { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; } = new HashSet<CalendarEventViewModel>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
