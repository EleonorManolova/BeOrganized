namespace BeOrganized.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;

    public class EventChangeViewModel
    {
        public EventViewModel EventModel { get; set; }

        public ICollection<CalendarEventViewModel> Calendars { get; set; } = new HashSet<CalendarEventViewModel>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
