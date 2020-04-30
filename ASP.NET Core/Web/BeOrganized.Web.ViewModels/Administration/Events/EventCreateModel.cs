namespace BeOrganized.Web.ViewModels.Administration.Events
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Events;

    public class EventCreateModel
    {
        public EventModel EventModel { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();

        public ICollection<Calendar> Calendars { get; set; } = new HashSet<Calendar>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
