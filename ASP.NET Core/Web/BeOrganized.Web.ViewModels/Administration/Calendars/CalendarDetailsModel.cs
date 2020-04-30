namespace BeOrganized.Web.ViewModels.Administration.Calendars
{
    using System;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class CalendarDetailsModel : IMapFrom<Calendar>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string UserUserName { get; set; }

        public string ColorName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
