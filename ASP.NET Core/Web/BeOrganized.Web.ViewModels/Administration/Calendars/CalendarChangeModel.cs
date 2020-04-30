namespace BeOrganized.Web.ViewModels.Administration.Calendars
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;

    public class CalendarChangeModel
    {
        public CalendarViewModel CalendarModel { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();

        public ICollection<Color> Colors { get; set; } = new HashSet<Color>();
    }
}
